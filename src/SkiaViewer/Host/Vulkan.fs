namespace FS.Skia.UI.SkiaViewer.Host

#nowarn "9"
#nowarn "51"
#nowarn "3261"
#nowarn "3391"
#nowarn "44"

open System
open System.IO
open System.Runtime.InteropServices
open System.Security.Cryptography
open System.Text
open Elmish
open Microsoft.FSharp.NativeInterop
open Silk.NET.Core
open Silk.NET.Core.Contexts
open Silk.NET.Core.Native
open Silk.NET.Input
open Silk.NET.Maths
open Silk.NET.Vulkan
open Silk.NET.Vulkan.Extensions.KHR
open Silk.NET.Windowing
open SkiaSharp
open FS.Skia.UI.Scene
// The shared scene painter (feature 063): both this interactive host and the
// image-evidence path delegate to `SceneRenderer.paintNode`.
open FS.Skia.UI.SkiaViewer
// Open the host namespace last so the host's own DiagnosticSeverity/DiagnosticStage/RenderDiagnostic
// (richer than the Scene package's) take precedence over the Scene-vocabulary names brought in above.
open FS.Skia.UI.SkiaViewer.Host

module VulkanResources =
    type ResourceCategory =
        | VulkanInstance
        | VulkanSurface
        | VulkanDevice
        | VulkanSwapchain
        | CommandPool
        | CommandBuffer
        | Fence
        | StagingBuffer
        | StagingMemory
        | SkiaGpu

    type OwnershipState =
        | Acquired
        | Transferred
        | Released

    type OwnedResource =
        { Id: string
          Category: ResourceCategory
          AcquireStage: string
          Owner: string
          TransferPoint: string option
          ReleaseAction: string
          State: OwnershipState }

    type ReleaseRecord =
        { Id: string
          Category: ResourceCategory
          Stage: string
          Order: int }

    type ResourceLedger =
        { Owned: OwnedResource list
          Released: ReleaseRecord list }

    let empty = { Owned = []; Released = [] }

    let acquire id category acquireStage owner releaseAction ledger =
        let resource =
            { Id = id
              Category = category
              AcquireStage = acquireStage
              Owner = owner
              TransferPoint = None
              ReleaseAction = releaseAction
              State = Acquired }

        { ledger with Owned = ledger.Owned @ [ resource ] }

    let transfer id transferPoint ledger =
        let update (resource: OwnedResource) =
            if resource.Id = id && resource.State <> Released then
                { resource with
                    State = Transferred
                    TransferPoint = Some transferPoint }
            else
                resource

        { ledger with Owned = ledger.Owned |> List.map update }

    let acquired (ledger: ResourceLedger) =
        ledger.Owned
        |> List.filter (fun resource -> resource.State <> Released)

    let releaseAll stage ledger =
        let releasable = acquired ledger |> List.rev

        let records =
            releasable
            |> List.mapi (fun index resource ->
                { Id = resource.Id
                  Category = resource.Category
                  Stage = stage
                  Order = index + 1 })

        let releasedIds = records |> List.map _.Id |> Set.ofList

        let owned =
            ledger.Owned
            |> List.map (fun resource ->
                if releasedIds.Contains resource.Id then
                    { resource with State = Released }
                else
                    resource)

        { Owned = owned
          Released = ledger.Released @ records },
        records

module VulkanStartup =
    type StartupStage =
        { Name: string
          Order: int
          Resource: VulkanResources.ResourceCategory option
          DiagnosticStage: string }

    type StartupFailureCase =
        { FailedStage: StartupStage
          AcquiredBeforeFailure: VulkanResources.OwnedResource list
          ExpectedReleaseOrder: VulkanResources.ResourceCategory list
          ObservedReleaseOrder: VulkanResources.ResourceCategory list
          DiagnosticStage: string
          DiagnosticCause: string
          Synthetic: bool }

    let stages =
        [ { Name = "create-vulkan-instance"
            Order = 10
            Resource = Some VulkanResources.VulkanInstance
            DiagnosticStage = "VulkanInstance" }
          { Name = "create-presentation-surface"
            Order = 20
            Resource = Some VulkanResources.VulkanSurface
            DiagnosticStage = "VulkanSurface" }
          { Name = "create-logical-device-and-queues"
            Order = 30
            Resource = Some VulkanResources.VulkanDevice
            DiagnosticStage = "VulkanDevice" }
          { Name = "create-swapchain-and-images"
            Order = 40
            Resource = Some VulkanResources.VulkanSwapchain
            DiagnosticStage = "VulkanSwapchain" }
          { Name = "create-command-pool"
            Order = 50
            Resource = Some VulkanResources.CommandPool
            DiagnosticStage = "FrameRender" }
          { Name = "allocate-command-buffers"
            Order = 60
            Resource = Some VulkanResources.CommandBuffer
            DiagnosticStage = "FrameRender" }
          { Name = "create-fence"
            Order = 70
            Resource = Some VulkanResources.Fence
            DiagnosticStage = "FrameRender" }
          { Name = "create-staging-buffer"
            Order = 80
            Resource = Some VulkanResources.StagingBuffer
            DiagnosticStage = "FrameRender" }
          { Name = "allocate-staging-memory"
            Order = 90
            Resource = Some VulkanResources.StagingMemory
            DiagnosticStage = "FrameRender" }
          { Name = "create-skia-gpu-context"
            Order = 100
            Resource = Some VulkanResources.SkiaGpu
            DiagnosticStage = "SkiaContext" } ]

    let stageByName name =
        stages |> List.tryFind (fun stage -> stage.Name = name)

    let releaseAction category =
        match category with
        | VulkanResources.VulkanInstance -> "vkDestroyInstance"
        | VulkanResources.VulkanSurface -> "vkDestroySurfaceKHR"
        | VulkanResources.VulkanDevice -> "vkDestroyDevice"
        | VulkanResources.VulkanSwapchain -> "vkDestroySwapchainKHR"
        | VulkanResources.CommandPool -> "vkDestroyCommandPool"
        | VulkanResources.CommandBuffer -> "free-with-command-pool"
        | VulkanResources.Fence -> "vkDestroyFence"
        | VulkanResources.StagingBuffer -> "vkDestroyBuffer"
        | VulkanResources.StagingMemory -> "vkFreeMemory"
        | VulkanResources.SkiaGpu -> "dispose-GRContext"

    let acquireStage ledger stage =
        match stage.Resource with
        | None -> ledger
        | Some category ->
            VulkanResources.acquire
                $"{stage.Name}-resource"
                category
                stage.Name
                "VulkanHost.run"
                (releaseAction category)
                ledger

    let acquireBefore failedStage =
        stages
        |> List.filter (fun stage -> stage.Order < failedStage.Order)
        |> List.fold acquireStage VulkanResources.empty

    let simulateFailure failedStageName =
        // SYNTHETIC: symbolic resource handles force each startup failure path; real native smoke is recorded in readiness/native-smoke.txt.
        let failedStage =
            stageByName failedStageName
            |> Option.defaultWith (fun () -> invalidArg (nameof failedStageName) $"Unknown startup stage: {failedStageName}")

        let ledger = acquireBefore failedStage
        let acquired = VulkanResources.acquired ledger
        let _, releases = VulkanResources.releaseAll failedStage.Name ledger

        { FailedStage = failedStage
          AcquiredBeforeFailure = acquired
          ExpectedReleaseOrder = acquired |> List.rev |> List.map _.Category
          ObservedReleaseOrder = releases |> List.map _.Category
          DiagnosticStage = failedStage.DiagnosticStage
          DiagnosticCause = $"{failedStage.Name} failed with synthetic native error"
          Synthetic = true }

    let simulateSuccessfulShutdown () =
        // SYNTHETIC: symbolic successful acquisition verifies idempotent reverse cleanup order without opening a real Vulkan device.
        let ledger = stages |> List.fold acquireStage VulkanResources.empty
        let _, firstRelease = VulkanResources.releaseAll "shutdown" ledger
        let afterFirst, _ = VulkanResources.releaseAll "shutdown" ledger
        let _, secondRelease = VulkanResources.releaseAll "shutdown" afterFirst

        firstRelease @ secondRelease

module VulkanHost =
    /// Feature 118 (DirectToSwapchain): persistent per-swapchain resources for the
    /// readback-free direct present path. Built lazily once per swapchain (rebuilt when a
    /// new SwapchainState is created on swapchain recreation, FR-006), disposed at shutdown.
    /// SkiaSharp 4.147 does not bind GRBackendSurfaceMutableState (mono/SkiaSharp #2191), so
    /// the GRBackendRenderTarget/SKSurface wrap is recreated per frame (cheap CPU object,
    /// far below the GPU→CPU readback it replaces); only these command/sync resources are
    /// cached per swapchain image index.
    type DirectPresentState() =
        /// Whether per-swapchain direct resources have been built (success or failure).
        member val Attempted = false with get, set
        /// Whether the direct path is usable; set false on any init/frame failure → the
        /// readback fallback path runs from that frame onward (FR-005).
        member val Available = false with get, set
        /// Whether the live present-mode diagnostic (FR-007) has been emitted once.
        member val Announced = false with get, set
        /// Per-swapchain command pool owning the present-layout transition buffers.
        member val CommandPool = CommandPool() with get, set
        /// Pre-recorded COLOR_ATTACHMENT_OPTIMAL → PRESENT_SRC_KHR transition, one per image.
        member val TransitionBuffers: CommandBuffer[] = [||] with get, set
        /// Per-image semaphore signalled by the transition submit, waited on by QueuePresent.
        member val PresentSemaphores: Semaphore[] = [||] with get, set

    type SwapchainState =
        { Swapchain: SwapchainKHR
          Format: Silk.NET.Vulkan.Format
          ImageUsage: ImageUsageFlags
          Extent: Extent2D
          /// Feature 118: direct-to-swapchain present resources (DirectToSwapchain mode only).
          Direct: DirectPresentState }

    type SkiaState =
        { Context: GRContext
          Extensions: GRVkExtensions
          Queue: Queue }

    type FrameSnapshot =
        { Width: int
          Height: int
          ColorType: SKColorType
          Pixels: byte[] }

    type StagingBuffer =
        { Buffer: Silk.NET.Vulkan.Buffer
          Memory: DeviceMemory
          Size: uint64 }

    let nullPtr<'T when 'T: unmanaged> : nativeptr<'T> =
        NativePtr.ofNativeInt 0n

    let checkResult stage operation result =
        if result = Result.Success then
            Ok()
        else
            let message =
                match stage with
                | FrameRender -> $"{operation} failed during Vulkan frame rendering. The viewer has no fallback renderer."
                | _ -> $"{operation} failed during Vulkan initialization. The viewer has no fallback renderer."

            Result.Error(
                Diagnostics.create
                    Fatal
                    stage
                    message
                    (Some(result.ToString()))
            )

    let trace configuration message =
        if configuration.Diagnostics.Verbose then
            Console.Error.WriteLine($"FS.Skia.UI VulkanHost: {message}")

    let toNativeSize (size: Size) =
        Vector2D<int>(size.Width, size.Height)

    let createWindow configuration =
        try
            let mutable options = WindowOptions.DefaultVulkan
            options.Title <- configuration.Title
            options.Size <- toNativeSize configuration.InitialSize
            options.IsVisible <- true
            options.API <- GraphicsAPI.DefaultVulkan
            options.FramesPerSecond <- configuration.TargetFrameRate |> Option.defaultValue 60 |> float
            options.UpdatesPerSecond <- options.FramesPerSecond
            // Carry window-startup intent (fullscreen / maximized / windowed-fullscreen
            // / borderless) into the live window before creation.
            match configuration.ConfigureWindow with
            | Some configure -> options <- configure options
            | None -> ()
            Ok(Window.Create options)
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Silk.NET window creation failed: {ex.Message}")

    let initializeWindow (window: IWindow) =
        try
            window.Initialize()

            if window.IsInitialized then
                Ok()
            else
                Result.Error(Diagnostics.startupFailed VulkanSurface "Silk.NET window did not initialize.")
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Silk.NET window initialization failed: {ex.Message}")

    let dispatchViewerEvent program dispatch event =
        program.EventMapper event
        |> Option.iter dispatch

    let addDisposable (items: ResizeArray<IDisposable>) dispose =
        items.Add
            { new IDisposable with
                member _.Dispose() = dispose () }

    let attachWindowEventMapping program (window: IWindow) onClosing dispatch =
        let disposables = ResizeArray<IDisposable>()

        let loadedHandler =
            Action(fun () -> dispatchViewerEvent program dispatch Loaded)

        window.add_Load loadedHandler
        addDisposable disposables (fun () -> window.remove_Load loadedHandler)

        let updateHandler =
            Action<float>(fun elapsedSeconds -> dispatchViewerEvent program dispatch (UpdateTick elapsedSeconds))

        window.add_Update updateHandler
        addDisposable disposables (fun () -> window.remove_Update updateHandler)

        let renderHandler =
            Action<float>(fun elapsedSeconds -> dispatchViewerEvent program dispatch (RenderTick elapsedSeconds))

        window.add_Render renderHandler
        addDisposable disposables (fun () -> window.remove_Render renderHandler)

        let resizeHandler =
            Action<Vector2D<int>>(fun size ->
                dispatchViewerEvent
                    program
                    dispatch
                    (Resized
                        { Width = size.X
                          Height = size.Y }))

        window.add_Resize resizeHandler
        addDisposable disposables (fun () -> window.remove_Resize resizeHandler)

        let closingHandler =
            Action(fun () ->
                onClosing ()
                dispatchViewerEvent program dispatch CloseRequested)

        window.add_Closing closingHandler
        addDisposable disposables (fun () -> window.remove_Closing closingHandler)

        { new IDisposable with
            member _.Dispose() =
                for disposable in Seq.rev disposables do
                    disposable.Dispose() }

    let attachInputEventMapping program (window: IWindow) dispatch =
        try
            let input = window.CreateInput()
            let disposables = ResizeArray<IDisposable>()

            addDisposable disposables (fun () -> input.Dispose())

            for keyboard in input.Keyboards do
                let keyDownHandler =
                    Action<IKeyboard, Key, int>(fun _ key _ -> dispatchViewerEvent program dispatch (KeyDown(key.ToString())))

                keyboard.add_KeyDown keyDownHandler
                addDisposable disposables (fun () -> keyboard.remove_KeyDown keyDownHandler)

                let keyUpHandler =
                    Action<IKeyboard, Key, int>(fun _ key _ -> dispatchViewerEvent program dispatch (KeyUp(key.ToString())))

                keyboard.add_KeyUp keyUpHandler
                addDisposable disposables (fun () -> keyboard.remove_KeyUp keyUpHandler)

            // 075 (FR-013): map the Silk.NET button identity to the host contract.
            let toViewerButton (button: MouseButton) =
                match button with
                | MouseButton.Left -> PrimaryButton
                | MouseButton.Right -> SecondaryButton
                | MouseButton.Middle -> MiddleButton
                | _ -> PrimaryButton

            for mouse in input.Mice do
                let pointerMoveHandler =
                    Action<IMouse, System.Numerics.Vector2>(fun _ position ->
                        dispatchViewerEvent program dispatch (PointerMoved(float position.X, float position.Y)))

                mouse.add_MouseMove pointerMoveHandler
                addDisposable disposables (fun () -> mouse.remove_MouseMove pointerMoveHandler)

                let pointerPressedHandler =
                    Action<IMouse, MouseButton>(fun mouse button ->
                        let position = mouse.Position
                        dispatchViewerEvent program dispatch (PointerPressed(float position.X, float position.Y, toViewerButton button)))

                mouse.add_MouseDown pointerPressedHandler
                addDisposable disposables (fun () -> mouse.remove_MouseDown pointerPressedHandler)

                let pointerReleasedHandler =
                    Action<IMouse, MouseButton>(fun mouse button ->
                        let position = mouse.Position
                        dispatchViewerEvent program dispatch (PointerReleased(float position.X, float position.Y, toViewerButton button)))

                mouse.add_MouseUp pointerReleasedHandler
                addDisposable disposables (fun () -> mouse.remove_MouseUp pointerReleasedHandler)

                // 075 (FR-014): mouse wheel → signed per-axis scroll delta.
                let pointerScrollHandler =
                    Action<IMouse, ScrollWheel>(fun mouse wheel ->
                        let position = mouse.Position
                        dispatchViewerEvent program dispatch (PointerScrolled(float position.X, float position.Y, float wheel.X, float wheel.Y)))

                mouse.add_Scroll pointerScrollHandler
                addDisposable disposables (fun () -> mouse.remove_Scroll pointerScrollHandler)

            // 075 (FR-007): window blur / focus-loss drives the deterministic
            // pointer-cancel path (mouse-leave is not exposed by this Silk.NET
            // version; focus loss is the available, reliable host trigger).
            let focusChangedHandler =
                Action<bool>(fun focused ->
                    if not focused then
                        dispatchViewerEvent program dispatch PointerExited)

            window.add_FocusChanged focusChangedHandler
            addDisposable disposables (fun () -> window.remove_FocusChanged focusChangedHandler)

            Ok
                { new IDisposable with
                    member _.Dispose() =
                        for disposable in Seq.rev disposables do
                            disposable.Dispose() }
        with ex ->
            Result.Error(Diagnostics.startupFailed PlatformCheck $"Silk.NET input event mapping failed: {ex.Message}")

    let getSurfaceSource (window: IWindow) =
        match box window with
        | :? IVkSurfaceSource as source when not (isNull source.VkSurface) -> Ok source.VkSurface
        | _ ->
            Result.Error(Diagnostics.startupFailed VulkanSurface "Silk.NET window did not expose a Vulkan surface source.")

    let copyRequiredExtensions (surface: IVkSurface) =
        try
            let mutable count = 0u
            let names = surface.GetRequiredExtensions(&count)

            if count = 0u || NativePtr.toNativeInt names = IntPtr.Zero then
                Result.Error(Diagnostics.startupFailed VulkanInstance "Windowing layer reported no required Vulkan surface extensions.")
            else
                Ok(names, count)
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanInstance $"Could not query required Vulkan surface extensions: {ex.Message}")

    let createInstance (vk: Vk) (extensionNames: nativeptr<nativeptr<byte>>) extensionCount =
        let mutable applicationInfo = ApplicationInfo()
        applicationInfo.SType <- StructureType.ApplicationInfo
        applicationInfo.ApiVersion <- Vk.Version11

        let mutable createInfo = InstanceCreateInfo()
        createInfo.SType <- StructureType.InstanceCreateInfo
        createInfo.PApplicationInfo <- &&applicationInfo
        createInfo.EnabledExtensionCount <- extensionCount
        createInfo.PpEnabledExtensionNames <- extensionNames

        let mutable instance = Instance()
        let result = vk.CreateInstance(&createInfo, nullPtr<AllocationCallbacks>, &instance)

        match checkResult VulkanInstance "vkCreateInstance" result with
        | Ok() -> Ok instance
        | Result.Error diagnostic -> Result.Error diagnostic

    let createSurface (surfaceSource: IVkSurface) (instance: Instance) =
        try
            let handle = surfaceSource.Create<AllocationCallbacks>(VkHandle(instance.Handle), nullPtr<AllocationCallbacks>)
            Ok(SurfaceKHR(Nullable<uint64>(handle.Handle)))
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Vulkan presentation surface creation failed: {ex.Message}")

    let enumeratePhysicalDevices (vk: Vk) instance =
        let mutable count = 0u
        let countResult = vk.EnumeratePhysicalDevices(instance, &count, nullPtr<PhysicalDevice>)

        match checkResult VulkanDevice "vkEnumeratePhysicalDevices(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanDevice "No Vulkan physical devices are available.")
        | Ok() ->
            let devices = Array.zeroCreate<PhysicalDevice> (int count)

            use devicesPtr = fixed devices
            let devicesResult = vk.EnumeratePhysicalDevices(instance, &count, devicesPtr)

            match checkResult VulkanDevice "vkEnumeratePhysicalDevices" devicesResult with
            | Ok() -> Ok devices
            | Result.Error diagnostic -> Result.Error diagnostic

    let findQueueFamily (vk: Vk) (surfaceExt: KhrSurface) surface (device: PhysicalDevice) =
        let mutable count = 0u
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, nullPtr<QueueFamilyProperties>)

        if count = 0u then
            None
        else
            let families = Array.zeroCreate<QueueFamilyProperties> (int count)

            use familiesPtr = fixed families
            vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, familiesPtr)

            families
            |> Array.mapi (fun index family -> uint32 index, family)
            |> Array.tryPick (fun (index, family) ->
                let hasGraphics = family.QueueCount > 0u && family.QueueFlags.HasFlag QueueFlags.GraphicsBit
                let mutable presentSupported = Bool32(false)
                let surfaceResult = surfaceExt.GetPhysicalDeviceSurfaceSupport(device, index, surface, &presentSupported)

                if hasGraphics && surfaceResult = Result.Success && presentSupported.Value <> 0u then
                    Some index
                else
                    None)

    let choosePhysicalDevice vk surfaceExt surface devices =
        devices
        |> Array.tryPick (fun device ->
            findQueueFamily vk surfaceExt surface device
            |> Option.map (fun queueFamily -> device, queueFamily))
        |> function
            | Some selection -> Ok selection
            | None ->
                Result.Error(
                    Diagnostics.startupFailed VulkanDevice "No Vulkan physical device supports graphics and presentation for this surface."
                )

    let createDevice (vk: Vk) (physicalDevice: PhysicalDevice) queueFamily =
        let mutable priority = 1.0f
        let mutable queueInfo = DeviceQueueCreateInfo()
        queueInfo.SType <- StructureType.DeviceQueueCreateInfo
        queueInfo.QueueFamilyIndex <- queueFamily
        queueInfo.QueueCount <- 1u
        queueInfo.PQueuePriorities <- &&priority

        let swapchainNameBytes = System.Text.Encoding.ASCII.GetBytes(KhrSwapchain.ExtensionName + "\u0000")

        use swapchainNamePtr = fixed swapchainNameBytes
        let mutable extensionNamePtr = swapchainNamePtr
        let mutable deviceCreateInfo = DeviceCreateInfo()
        deviceCreateInfo.SType <- StructureType.DeviceCreateInfo
        deviceCreateInfo.QueueCreateInfoCount <- 1u
        deviceCreateInfo.PQueueCreateInfos <- &&queueInfo
        deviceCreateInfo.EnabledExtensionCount <- 1u
        deviceCreateInfo.PpEnabledExtensionNames <- &&extensionNamePtr

        let mutable device = Device()
        let result = vk.CreateDevice(physicalDevice, &deviceCreateInfo, nullPtr<AllocationCallbacks>, &device)

        match checkResult VulkanDevice "vkCreateDevice" result with
        | Ok() -> Ok device
        | Result.Error diagnostic -> Result.Error diagnostic

    let getSurfaceFormats (surfaceExt: KhrSurface) physicalDevice surface =
        let mutable count = 0u
        let countResult = surfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, nullPtr<SurfaceFormatKHR>)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceFormatsKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanSwapchain "The Vulkan surface exposes no image formats.")
        | Ok() ->
            let formats = Array.zeroCreate<SurfaceFormatKHR> (int count)

            use formatsPtr = fixed formats
            let result = surfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, formatsPtr)

            match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceFormatsKHR" result with
            | Ok() -> Ok formats
            | Result.Error diagnostic -> Result.Error diagnostic

    let getPresentModes (surfaceExt: KhrSurface) physicalDevice surface =
        let mutable count = 0u
        let countResult = surfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, nullPtr<PresentModeKHR>)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfacePresentModesKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanSwapchain "The Vulkan surface exposes no present modes.")
        | Ok() ->
            let modes = Array.zeroCreate<PresentModeKHR> (int count)

            use modesPtr = fixed modes
            let result = surfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, modesPtr)

            match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfacePresentModesKHR" result with
            | Ok() -> Ok modes
            | Result.Error diagnostic -> Result.Error diagnostic

    let chooseExtent configuration (capabilities: SurfaceCapabilitiesKHR) =
        if capabilities.CurrentExtent.Width <> UInt32.MaxValue then
            capabilities.CurrentExtent
        else
            let clamp (minValue: uint32) (maxValue: uint32) (value: uint32) =
                Math.Max(minValue, Math.Min(maxValue, value))

            let mutable extent = Extent2D()
            extent.Width <- clamp capabilities.MinImageExtent.Width capabilities.MaxImageExtent.Width (uint32 configuration.InitialSize.Width)
            extent.Height <- clamp capabilities.MinImageExtent.Height capabilities.MaxImageExtent.Height (uint32 configuration.InitialSize.Height)
            extent

    let createSwapchain configuration (surfaceExt: KhrSurface) (swapchainExt: KhrSwapchain) physicalDevice device surface =
        let mutable capabilities = SurfaceCapabilitiesKHR()
        let capabilitiesResult = surfaceExt.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, &capabilities)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceCapabilitiesKHR" capabilitiesResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            match getSurfaceFormats surfaceExt physicalDevice surface, getPresentModes surfaceExt physicalDevice surface with
            | Ok formats, Ok presentModes ->
                let formatSummary =
                    formats
                    |> Array.map (fun f -> sprintf "%O/%O" f.Format f.ColorSpace)
                    |> String.concat ", "

                trace configuration $"surface capabilities supportedUsage={capabilities.SupportedUsageFlags} minImages={capabilities.MinImageCount} maxImages={capabilities.MaxImageCount}"
                trace configuration $"surface formats={formatSummary}"

                let format: SurfaceFormatKHR =
                    formats
                    |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.B8G8R8A8Unorm)
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.R8G8B8A8Unorm))
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.B8G8R8A8Srgb))
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.R8G8B8A8Srgb))
                    |> Option.defaultValue formats[0]

                trace configuration $"selected swapchain format={format.Format} colorSpace={format.ColorSpace}"

                let presentMode =
                    presentModes
                    |> Array.tryFind ((=) PresentModeKHR.MailboxKhr)
                    |> Option.defaultValue PresentModeKHR.FifoKhr

                let mutable imageCount = capabilities.MinImageCount + 1u

                if capabilities.MaxImageCount > 0u && imageCount > capabilities.MaxImageCount then
                    imageCount <- capabilities.MaxImageCount

                let imageUsage = ImageUsageFlags.ColorAttachmentBit
                let mutable createInfo = SwapchainCreateInfoKHR()
                createInfo.SType <- StructureType.SwapchainCreateInfoKhr
                createInfo.Surface <- surface
                createInfo.MinImageCount <- imageCount
                createInfo.ImageFormat <- format.Format
                createInfo.ImageColorSpace <- format.ColorSpace
                createInfo.ImageExtent <- chooseExtent configuration capabilities
                createInfo.ImageArrayLayers <- 1u
                createInfo.ImageUsage <- imageUsage
                createInfo.ImageSharingMode <- SharingMode.Exclusive
                createInfo.PreTransform <- capabilities.CurrentTransform
                createInfo.CompositeAlpha <- CompositeAlphaFlagsKHR.OpaqueBitKhr
                createInfo.PresentMode <- presentMode
                createInfo.Clipped <- Bool32(true)

                let mutable swapchain = SwapchainKHR()
                let result = swapchainExt.CreateSwapchain(device, &createInfo, nullPtr<AllocationCallbacks>, &swapchain)

                match checkResult VulkanSwapchain "vkCreateSwapchainKHR" result with
                | Ok() ->
                    Ok
                        { Swapchain = swapchain
                          Format = format.Format
                          ImageUsage = imageUsage
                          Extent = createInfo.ImageExtent
                          Direct = DirectPresentState() }
                | Result.Error diagnostic -> Result.Error diagnostic
            | Result.Error diagnostic, _ -> Result.Error diagnostic
            | _, Result.Error diagnostic -> Result.Error diagnostic

    let createSkiaContext configuration (vk: Vk) (instance: Instance) (physicalDevice: PhysicalDevice) (device: Device) (queueFamily: uint32) =
        try
            trace configuration "creating device queue"
            let queue = vk.GetDeviceQueue(device, queueFamily, 0u)
            trace configuration $"device queue handle={queue.Handle}"

            let getProcAddress =
                GRVkGetProcedureAddressDelegate(fun name instanceHandle deviceHandle ->
                    let handle =
                        if deviceHandle <> IntPtr.Zero then
                            let fn = vk.GetDeviceProcAddr(Device(Nullable<IntPtr>(deviceHandle)), name)
                            fn.Handle
                        else
                            let fn = vk.GetInstanceProcAddr(Instance(Nullable<IntPtr>(instanceHandle)), name)
                            fn.Handle

                    if handle = IntPtr.Zero then
                        trace configuration $"Vulkan proc address not found: {name}"

                    handle)

            trace configuration $"creating Skia Vulkan extension table instance={instance.Handle} physicalDevice={physicalDevice.Handle} device={device.Handle}"
            let extensions =
                GRVkExtensions.Create(getProcAddress, instance.Handle, physicalDevice.Handle, [| KhrSurface.ExtensionName |], [| KhrSwapchain.ExtensionName |])

            trace configuration "creating Skia Vulkan backend context"
            let backend = new GRVkBackendContext()
            backend.VkInstance <- instance.Handle
            backend.VkPhysicalDevice <- physicalDevice.Handle
            backend.VkDevice <- device.Handle
            backend.VkQueue <- queue.Handle
            backend.GraphicsQueueIndex <- queueFamily
            backend.MaxAPIVersion <- Vk.Version11
            backend.Extensions <- extensions
            backend.GetProcedureAddress <- getProcAddress

            trace configuration "creating Skia GRContext"
            let context = GRContext.CreateVulkan backend

            if isNull context then
                Result.Error(Diagnostics.startupFailed SkiaContext "SkiaSharp did not create a Vulkan GPU context.")
            else
                Ok
                    { Context = context
                      Extensions = extensions
                      Queue = queue }
        with ex ->
            Result.Error(Diagnostics.startupFailed SkiaContext $"SkiaSharp Vulkan GPU context creation failed: {ex.Message}")

    let getSwapchainImages (swapchainExt: KhrSwapchain) device swapchain =
        let mutable count = 0u
        let countResult = swapchainExt.GetSwapchainImages(device, swapchain, &count, nullPtr<Image>)

        match checkResult FrameRender "vkGetSwapchainImagesKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.create Fatal FrameRender "Swapchain exposed no renderable images." None)
        | Ok() ->
            let images = Array.zeroCreate<Image> (int count)
            use imagesPtr = fixed images
            let result = swapchainExt.GetSwapchainImages(device, swapchain, &count, imagesPtr)

            match checkResult FrameRender "vkGetSwapchainImagesKHR" result with
            | Ok() -> Ok images
            | Result.Error diagnostic -> Result.Error diagnostic

    let createFence (vk: Vk) (device: Device) =
        let mutable createInfo = FenceCreateInfo()
        createInfo.SType <- StructureType.FenceCreateInfo

        let mutable fence = Fence()
        let result = vk.CreateFence(device, &createInfo, nullPtr<AllocationCallbacks>, &fence)

        match checkResult FrameRender "vkCreateFence" result with
        | Ok() -> Ok fence
        | Result.Error diagnostic -> Result.Error diagnostic

    let acquireImage (vk: Vk) (swapchainExt: KhrSwapchain) (device: Device) swapchain fence =
        let mutable imageIndex = 0u
        let result = swapchainExt.AcquireNextImage(device, swapchain, UInt64.MaxValue, Semaphore(Nullable<uint64>()), fence, &imageIndex)

        match checkResult FrameRender "vkAcquireNextImageKHR" result with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            let mutable acquireFence = fence
            let waitResult = vk.WaitForFences(device, 1u, &acquireFence, Bool32(true), UInt64.MaxValue)

            match checkResult FrameRender "vkWaitForFences" waitResult with
            | Ok() -> Ok imageIndex
            | Result.Error diagnostic -> Result.Error diagnostic

    let transitionBarrier image oldLayout newLayout srcAccess dstAccess =
        let mutable range = ImageSubresourceRange()
        range.AspectMask <- ImageAspectFlags.ColorBit
        range.BaseMipLevel <- 0u
        range.LevelCount <- 1u
        range.BaseArrayLayer <- 0u
        range.LayerCount <- 1u

        let mutable barrier = ImageMemoryBarrier()
        barrier.SType <- StructureType.ImageMemoryBarrier
        barrier.OldLayout <- oldLayout
        barrier.NewLayout <- newLayout
        barrier.SrcQueueFamilyIndex <- UInt32.MaxValue
        barrier.DstQueueFamilyIndex <- UInt32.MaxValue
        barrier.Image <- image
        barrier.SubresourceRange <- range
        barrier.SrcAccessMask <- srcAccess
        barrier.DstAccessMask <- dstAccess
        barrier

    let drawScene scene (canvas: SKCanvas) =
        // Feature 063 (FR-001): delegate to the single shared exhaustive painter.
        scene.Nodes |> List.iter (SceneRenderer.paintNode canvas)

    let colorTypeForFormat format =
        match format with
        | Silk.NET.Vulkan.Format.B8G8R8A8Unorm
        | Silk.NET.Vulkan.Format.B8G8R8A8Srgb -> SKColorType.Bgra8888
        | Silk.NET.Vulkan.Format.R8G8B8A8Unorm
        | Silk.NET.Vulkan.Format.R8G8B8A8Srgb -> SKColorType.Rgba8888
        | _ -> SKColorType.Bgra8888

    let bind result next =
        match result with
        | Ok value -> next value
        | Result.Error diagnostic -> Result.Error diagnostic

    type ResultBuilder() =
        member _.Bind(result, next) = bind result next
        member _.Return value = Ok value
        member _.ReturnFrom result = result
        member _.Zero() = Ok()

    let result = ResultBuilder()

    let findMemoryType (vk: Vk) physicalDevice typeFilter requiredFlags =
        let mutable properties = PhysicalDeviceMemoryProperties()
        vk.GetPhysicalDeviceMemoryProperties(physicalDevice, &properties)

        [ 0u .. properties.MemoryTypeCount - 1u ]
        |> List.tryFind (fun index ->
            let memoryType = properties.MemoryTypes[int index]
            let supported = (typeFilter &&& (1u <<< int index)) <> 0u
            supported && memoryType.PropertyFlags.HasFlag requiredFlags)

    let createStagingBuffer configuration (vk: Vk) physicalDevice device (pixels: byte[]) =
        let size = uint64 pixels.Length
        let mutable bufferInfo = BufferCreateInfo()
        bufferInfo.SType <- StructureType.BufferCreateInfo
        bufferInfo.Size <- size
        bufferInfo.Usage <- BufferUsageFlags.TransferSrcBit
        bufferInfo.SharingMode <- SharingMode.Exclusive

        let mutable buffer = Silk.NET.Vulkan.Buffer()
        let bufferResult = vk.CreateBuffer(device, &bufferInfo, nullPtr<AllocationCallbacks>, &buffer)

        match checkResult FrameRender "vkCreateBuffer(staging)" bufferResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            let mutable memory = DeviceMemory()

            try
                let mutable requirements = MemoryRequirements()
                vk.GetBufferMemoryRequirements(device, buffer, &requirements)

                match findMemoryType vk physicalDevice requirements.MemoryTypeBits (MemoryPropertyFlags.HostVisibleBit ||| MemoryPropertyFlags.HostCoherentBit) with
                | None ->
                    Result.Error(Diagnostics.create Error FrameRender "No host-visible Vulkan memory type is available for Skia frame upload." None)
                | Some memoryTypeIndex ->
                    let mutable allocateInfo = MemoryAllocateInfo()
                    allocateInfo.SType <- StructureType.MemoryAllocateInfo
                    allocateInfo.AllocationSize <- requirements.Size
                    allocateInfo.MemoryTypeIndex <- memoryTypeIndex

                    let allocateResult = vk.AllocateMemory(device, &allocateInfo, nullPtr<AllocationCallbacks>, &memory)

                    match checkResult FrameRender "vkAllocateMemory(staging)" allocateResult with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() ->
                        let bindResult = vk.BindBufferMemory(device, buffer, memory, 0UL)

                        match checkResult FrameRender "vkBindBufferMemory(staging)" bindResult with
                        | Result.Error diagnostic -> Result.Error diagnostic
                        | Ok() ->
                            let mutable mapped: voidptr = NativePtr.toVoidPtr (NativePtr.ofNativeInt<byte> 0n)
                            let mapResult = vk.MapMemory(device, memory, 0UL, size, Unchecked.defaultof<MemoryMapFlags>, &mapped)

                            match checkResult FrameRender "vkMapMemory(staging)" mapResult with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok() ->
                                Marshal.Copy(pixels, 0, NativePtr.toNativeInt (NativePtr.ofVoidPtr<byte> mapped), pixels.Length)
                                vk.UnmapMemory(device, memory)
                                Ok { Buffer = buffer; Memory = memory; Size = size }
            with ex ->
                if memory.Handle <> 0UL then
                    vk.FreeMemory(device, memory, nullPtr<AllocationCallbacks>)

                if buffer.Handle <> 0UL then
                    vk.DestroyBuffer(device, buffer, nullPtr<AllocationCallbacks>)

                Result.Error(Diagnostics.create Error FrameRender "Vulkan staging buffer creation failed." (Some ex.Message))

    let destroyStagingBuffer (vk: Vk) device staging =
        if staging.Buffer.Handle <> 0UL then
            vk.DestroyBuffer(device, staging.Buffer, nullPtr<AllocationCallbacks>)

        if staging.Memory.Handle <> 0UL then
            vk.FreeMemory(device, staging.Memory, nullPtr<AllocationCallbacks>)

    let renderSceneToPixels configuration (skiaState: SkiaState) (extent: Extent2D) colorType scene =
        try
            let width = int extent.Width
            let height = int extent.Height
            let imageInfo = SKImageInfo(width, height, colorType, SKAlphaType.Premul)

            use surface =
                SKSurface.Create(skiaState.Context, true, imageInfo, 1, GRSurfaceOrigin.TopLeft)

            if isNull surface then
                Result.Error(Diagnostics.create Error FrameRender "SkiaSharp did not create an offscreen Vulkan surface for scene rendering." None)
            else
                let clear =
                    configuration.ClearColor
                    |> Option.defaultValue Colors.black
                    |> SceneRenderer.skColor

                trace configuration "drawing model-derived scene into Skia Vulkan surface"
                surface.Canvas.Clear clear
                drawScene scene surface.Canvas
                surface.Canvas.Flush()
                surface.Flush()
                skiaState.Context.Flush()
                skiaState.Context.Submit(true)

                let rowBytes = imageInfo.RowBytes
                let pixels = Array.zeroCreate<byte> (rowBytes * height)
                let handle = GCHandle.Alloc(pixels, GCHandleType.Pinned)

                try
                    let ok = surface.ReadPixels(imageInfo, handle.AddrOfPinnedObject(), rowBytes, 0, 0)

                    if ok then
                        Ok pixels
                    else
                        Result.Error(Diagnostics.create Error FrameRender "SkiaSharp could not read the rendered scene pixels." None)
                finally
                    handle.Free()
        with ex ->
            Result.Error(Diagnostics.create Error FrameRender "Skia scene rendering failed." (Some ex.Message))

    let copyPixelsToSwapchainImage configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) queueFamily queue image imageIndex colorType pixels =
        bind (createStagingBuffer configuration vk physicalDevice device pixels) (fun staging ->
                try
                    let mutable poolInfo = CommandPoolCreateInfo()
                    poolInfo.SType <- StructureType.CommandPoolCreateInfo
                    poolInfo.Flags <- CommandPoolCreateFlags.ResetCommandBufferBit
                    poolInfo.QueueFamilyIndex <- queueFamily

                    let mutable commandPool = CommandPool()
                    let poolResult = vk.CreateCommandPool(device, &poolInfo, nullPtr<AllocationCallbacks>, &commandPool)

                    match checkResult FrameRender "vkCreateCommandPool(frame upload)" poolResult with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() ->
                        try
                            let mutable allocInfo = CommandBufferAllocateInfo()
                            allocInfo.SType <- StructureType.CommandBufferAllocateInfo
                            allocInfo.CommandPool <- commandPool
                            allocInfo.Level <- CommandBufferLevel.Primary
                            allocInfo.CommandBufferCount <- 1u

                            let mutable commandBuffer = CommandBuffer()
                            let allocResult = vk.AllocateCommandBuffers(device, &allocInfo, &commandBuffer)

                            match checkResult FrameRender "vkAllocateCommandBuffers(frame upload)" allocResult with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok() ->
                                let mutable beginInfo = CommandBufferBeginInfo()
                                beginInfo.SType <- StructureType.CommandBufferBeginInfo
                                beginInfo.Flags <- CommandBufferUsageFlags.OneTimeSubmitBit

                                let beginResult = vk.BeginCommandBuffer(commandBuffer, &beginInfo)

                                match checkResult FrameRender "vkBeginCommandBuffer(frame upload)" beginResult with
                                | Result.Error diagnostic -> Result.Error diagnostic
                                | Ok() ->
                                    let mutable toTransfer =
                                        transitionBarrier
                                            image
                                            ImageLayout.Undefined
                                            ImageLayout.TransferDstOptimal
                                            AccessFlags.None
                                            AccessFlags.TransferWriteBit

                                    vk.CmdPipelineBarrier(
                                        commandBuffer,
                                        PipelineStageFlags.TopOfPipeBit,
                                        PipelineStageFlags.TransferBit,
                                        DependencyFlags.None,
                                        0u,
                                        nullPtr<MemoryBarrier>,
                                        0u,
                                        nullPtr<BufferMemoryBarrier>,
                                        1u,
                                        &toTransfer
                                    )

                                    let mutable subresource = ImageSubresourceLayers()
                                    subresource.AspectMask <- ImageAspectFlags.ColorBit
                                    subresource.MipLevel <- 0u
                                    subresource.BaseArrayLayer <- 0u
                                    subresource.LayerCount <- 1u

                                    let mutable copyRegion = BufferImageCopy()
                                    copyRegion.BufferOffset <- 0UL
                                    copyRegion.BufferRowLength <- 0u
                                    copyRegion.BufferImageHeight <- 0u
                                    copyRegion.ImageSubresource <- subresource
                                    copyRegion.ImageOffset <- Offset3D(0, 0, 0)
                                    copyRegion.ImageExtent <- Extent3D(swapchainState.Extent.Width, swapchainState.Extent.Height, 1u)

                                    vk.CmdCopyBufferToImage(commandBuffer, staging.Buffer, image, ImageLayout.TransferDstOptimal, 1u, &copyRegion)

                                    let mutable toPresent =
                                        transitionBarrier
                                            image
                                            ImageLayout.TransferDstOptimal
                                            ImageLayout.PresentSrcKhr
                                            AccessFlags.TransferWriteBit
                                            AccessFlags.None

                                    vk.CmdPipelineBarrier(
                                        commandBuffer,
                                        PipelineStageFlags.TransferBit,
                                        PipelineStageFlags.BottomOfPipeBit,
                                        DependencyFlags.None,
                                        0u,
                                        nullPtr<MemoryBarrier>,
                                        0u,
                                        nullPtr<BufferMemoryBarrier>,
                                        1u,
                                        &toPresent
                                    )

                                    let endResult = vk.EndCommandBuffer(commandBuffer)

                                    match checkResult FrameRender "vkEndCommandBuffer(frame upload)" endResult with
                                    | Result.Error diagnostic -> Result.Error diagnostic
                                    | Ok() ->
                                        let mutable submitInfo = SubmitInfo()
                                        submitInfo.SType <- StructureType.SubmitInfo
                                        submitInfo.CommandBufferCount <- 1u
                                        submitInfo.PCommandBuffers <- &&commandBuffer

                                        let submitResult = vk.QueueSubmit(queue, 1u, &submitInfo, Fence(Nullable<uint64>()))

                                        match checkResult FrameRender "vkQueueSubmit(frame upload)" submitResult with
                                        | Result.Error diagnostic -> Result.Error diagnostic
                                        | Ok() ->
                                            let waitResult = vk.QueueWaitIdle(queue)

                                            match checkResult FrameRender "vkQueueWaitIdle(frame upload)" waitResult with
                                            | Result.Error diagnostic -> Result.Error diagnostic
                                            | Ok() ->
                                                let mutable presentInfo = PresentInfoKHR()
                                                let mutable presentedSwapchain = swapchainState.Swapchain
                                                let mutable presentImageIndex = imageIndex
                                                presentInfo.SType <- StructureType.PresentInfoKhr
                                                presentInfo.SwapchainCount <- 1u
                                                presentInfo.PSwapchains <- &&presentedSwapchain
                                                presentInfo.PImageIndices <- &&presentImageIndex

                                                let presentResult = swapchainExt.QueuePresent(queue, &presentInfo)

                                                match checkResult FrameRender "vkQueuePresentKHR" presentResult with
                                                | Ok() -> Ok()
                                                | Result.Error diagnostic -> Result.Error diagnostic
                        finally
                            if commandPool.Handle <> 0UL then
                                vk.DestroyCommandPool(device, commandPool, nullPtr<AllocationCallbacks>)
                finally
                    destroyStagingBuffer vk device staging)

    // OffscreenReadback present path (the default, unchanged): offscreen render → GPU→CPU
    // readback → per-frame staging upload → vkQueueWaitIdle → present. Byte-identical to the
    // pre-feature baseline (FR-001/SC-001).
    let renderFrameReadback configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) (skiaState: SkiaState) queueFamily scene =
        try
            trace configuration "querying swapchain images"
            match getSwapchainImages swapchainExt device swapchainState.Swapchain with
            | Result.Error diagnostic -> Result.Error diagnostic
            | Ok images ->
                match createFence vk device with
                | Result.Error diagnostic -> Result.Error diagnostic
                | Ok fence ->
                    try
                        match acquireImage vk swapchainExt device swapchainState.Swapchain fence with
                        | Result.Error diagnostic -> Result.Error diagnostic
                        | Ok imageIndex ->
                            let image = images[int imageIndex]
                            let colorType = colorTypeForFormat swapchainState.Format
                            trace configuration $"rendering Skia scene for swapchain image index={imageIndex} format={swapchainState.Format} colorType={colorType}"
                            trace configuration $"skia maxSampleCount colorType={colorType} count={skiaState.Context.GetMaxSurfaceSampleCount(colorType)}"
                            trace configuration $"skia context abandoned={skiaState.Context.IsAbandoned} maxRenderTargetSize={skiaState.Context.MaxRenderTargetSize}"
                            bind (renderSceneToPixels configuration skiaState swapchainState.Extent colorType scene) (fun pixels ->
                                bind (copyPixelsToSwapchainImage configuration vk swapchainExt physicalDevice device swapchainState queueFamily skiaState.Queue image imageIndex colorType pixels) (fun () ->
                                    Ok
                                        { Width = int swapchainState.Extent.Width
                                          Height = int swapchainState.Extent.Height
                                          ColorType = colorType
                                          Pixels = pixels }))
                    finally
                        if fence.Handle <> 0UL then
                            vk.DestroyFence(device, fence, nullPtr<AllocationCallbacks>)
        with ex ->
            Result.Error(Diagnostics.frameRenderFailed ex.Message)

    // --- Feature 118: DirectToSwapchain present path ---------------------------------------

    let createSemaphore (vk: Vk) (device: Device) =
        let mutable createInfo = SemaphoreCreateInfo()
        createInfo.SType <- StructureType.SemaphoreCreateInfo
        let mutable semaphore = Semaphore()
        let result = vk.CreateSemaphore(device, &createInfo, nullPtr<AllocationCallbacks>, &semaphore)

        match checkResult VulkanSwapchain "vkCreateSemaphore(direct present)" result with
        | Ok() -> Ok semaphore
        | Result.Error diagnostic -> Result.Error diagnostic

    // Build the GRVkImageInfo describing an acquired swapchain image for Skia. ImageLayout is
    // UNDEFINED — a valid barrier source from any prior layout that discards stale contents,
    // correct because each direct frame is a full Clear+redraw.
    let swapchainImageInfo (swapchainState: SwapchainState) queueFamily (image: Image) =
        let mutable imageInfo = GRVkImageInfo()
        imageInfo.Image <- image.Handle
        imageInfo.Format <- uint (int swapchainState.Format)
        imageInfo.ImageTiling <- uint (int ImageTiling.Optimal)
        imageInfo.ImageLayout <- uint (int ImageLayout.Undefined)
        imageInfo.ImageUsageFlags <- uint (int swapchainState.ImageUsage)
        imageInfo.SampleCount <- 1u
        imageInfo.LevelCount <- 1u
        imageInfo.CurrentQueueFamily <- queueFamily
        imageInfo.SharingMode <- uint (int SharingMode.Exclusive)
        imageInfo

    // Probe whether SkiaSharp can wrap an acquired swapchain image as a renderable SKSurface.
    //
    // KNOWN LIMITATION (verified): SkiaSharp's managed binding (incl. the newest 4.147 preview)
    // does NOT support creating an SKSurface from a Vulkan-backed GRBackendRenderTarget —
    // SKSurface.Create returns null even when GRBackendRenderTarget.IsValid is true and the
    // VkImage handle/format are correct (mono/SkiaSharp #1502, open since 2020; the layout
    // interop #2191 is likewise unbound). The readback-free DirectToSwapchain present therefore
    // cannot be built on SkiaSharp; this probe detects it once and the path degrades to the
    // proven OffscreenReadback fallback (FR-005). See
    // specs/118-backend-host-review/feedback/skiasharp-vulkan-layout-api.md and
    // readiness/audit/present-path-audit.md (OpenGL-backend resolution).
    let probeDirectWrap (skiaState: SkiaState) (swapchainState: SwapchainState) queueFamily (image: Image) colorType =
        try
            let width = int swapchainState.Extent.Width
            let height = int swapchainState.Extent.Height
            let imageInfo = swapchainImageInfo swapchainState queueFamily image
            use rt = new GRBackendRenderTarget(width, height, 1, imageInfo)
            use surface = SKSurface.Create(skiaState.Context, rt, GRSurfaceOrigin.TopLeft, colorType)

            if rt.IsValid && not (isNull surface) then
                Ok()
            else
                Result.Error(
                    Diagnostics.create
                        Warning
                        VulkanSwapchain
                        "SkiaSharp cannot wrap a Vulkan swapchain image as an SKSurface (managed-binding limitation, mono/SkiaSharp #1502); DirectToSwapchain is unavailable and the viewer uses the OffscreenReadback present path."
                        (Some(sprintf "GRBackendRenderTarget.IsValid=%b SKSurface.Create=%s" rt.IsValid (if isNull surface then "null" else "ok")))
                )
        with ex ->
            Result.Error(Diagnostics.create Warning VulkanSwapchain "Direct-to-swapchain wrap probe threw." (Some ex.Message))

    // Build the persistent per-swapchain direct resources once: a command pool plus a
    // pre-recorded COLOR_ATTACHMENT_OPTIMAL → PRESENT_SRC_KHR transition buffer and a present
    // semaphore per swapchain image. Skia leaves the wrapped render target in
    // COLOR_ATTACHMENT_OPTIMAL after flush, so the same transition is valid every frame; a
    // reacquire of an image implies its prior present (and thus prior transition) completed,
    // so the pre-recorded buffer/semaphore are safely resubmitted without a reuse fence.
    // Probes wrap capability first so no resources are allocated when the binding can't wrap.
    let initDirectPresent configuration (vk: Vk) (skiaState: SkiaState) device queueFamily (swapchainState: SwapchainState) (images: Image[]) colorType =
      match (if images.Length = 0 then Result.Error(Diagnostics.create Warning VulkanSwapchain "Swapchain exposed no images for direct present." None) else probeDirectWrap skiaState swapchainState queueFamily images[0] colorType) with
      | Result.Error diagnostic -> Result.Error diagnostic
      | Ok() ->
        try
            let direct = swapchainState.Direct
            let mutable poolInfo = CommandPoolCreateInfo()
            poolInfo.SType <- StructureType.CommandPoolCreateInfo
            poolInfo.QueueFamilyIndex <- queueFamily
            let mutable commandPool = CommandPool()
            let poolResult = vk.CreateCommandPool(device, &poolInfo, nullPtr<AllocationCallbacks>, &commandPool)

            match checkResult VulkanSwapchain "vkCreateCommandPool(direct present)" poolResult with
            | Result.Error diagnostic -> Result.Error diagnostic
            | Ok() ->
                direct.CommandPool <- commandPool
                let buffers = Array.zeroCreate<CommandBuffer> images.Length
                let semaphores = Array.zeroCreate<Semaphore> images.Length

                let rec buildEach index =
                    if index >= images.Length then
                        Ok()
                    else
                        let mutable allocInfo = CommandBufferAllocateInfo()
                        allocInfo.SType <- StructureType.CommandBufferAllocateInfo
                        allocInfo.CommandPool <- commandPool
                        allocInfo.Level <- CommandBufferLevel.Primary
                        allocInfo.CommandBufferCount <- 1u
                        let mutable commandBuffer = CommandBuffer()
                        let allocResult = vk.AllocateCommandBuffers(device, &allocInfo, &commandBuffer)

                        match checkResult VulkanSwapchain "vkAllocateCommandBuffers(direct present)" allocResult with
                        | Result.Error diagnostic -> Result.Error diagnostic
                        | Ok() ->
                            // Record (once, resubmittable — no OneTimeSubmit flag) the
                            // present-layout transition for this image.
                            let mutable beginInfo = CommandBufferBeginInfo()
                            beginInfo.SType <- StructureType.CommandBufferBeginInfo
                            let beginResult = vk.BeginCommandBuffer(commandBuffer, &beginInfo)

                            match checkResult VulkanSwapchain "vkBeginCommandBuffer(direct present)" beginResult with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok() ->
                                let mutable toPresent =
                                    transitionBarrier
                                        images[index]
                                        ImageLayout.ColorAttachmentOptimal
                                        ImageLayout.PresentSrcKhr
                                        AccessFlags.ColorAttachmentWriteBit
                                        AccessFlags.None

                                vk.CmdPipelineBarrier(
                                    commandBuffer,
                                    PipelineStageFlags.ColorAttachmentOutputBit,
                                    PipelineStageFlags.BottomOfPipeBit,
                                    DependencyFlags.None,
                                    0u,
                                    nullPtr<MemoryBarrier>,
                                    0u,
                                    nullPtr<BufferMemoryBarrier>,
                                    1u,
                                    &toPresent
                                )

                                match checkResult VulkanSwapchain "vkEndCommandBuffer(direct present)" (vk.EndCommandBuffer commandBuffer) with
                                | Result.Error diagnostic -> Result.Error diagnostic
                                | Ok() ->
                                    match createSemaphore vk device with
                                    | Result.Error diagnostic -> Result.Error diagnostic
                                    | Ok semaphore ->
                                        buffers[index] <- commandBuffer
                                        semaphores[index] <- semaphore
                                        buildEach (index + 1)

                match buildEach 0 with
                | Result.Error diagnostic -> Result.Error diagnostic
                | Ok() ->
                    direct.TransitionBuffers <- buffers
                    direct.PresentSemaphores <- semaphores
                    Ok()
        with ex ->
            Result.Error(Diagnostics.create Error VulkanSwapchain "Direct-to-swapchain present initialization failed." (Some ex.Message))

    let disposeDirectPresent (vk: Vk) device (direct: DirectPresentState) =
        for semaphore in direct.PresentSemaphores do
            if semaphore.Handle <> 0UL then
                vk.DestroySemaphore(device, semaphore, nullPtr<AllocationCallbacks>)

        if direct.CommandPool.Handle <> 0UL then
            vk.DestroyCommandPool(device, direct.CommandPool, nullPtr<AllocationCallbacks>)

        direct.PresentSemaphores <- [||]
        direct.TransitionBuffers <- [||]
        direct.CommandPool <- CommandPool()

    // Render the scene straight onto the acquired swapchain image, transition it to
    // PRESENT_SRC_KHR with the pre-recorded buffer, and present — no readback, no per-frame
    // staging buffer/command pool, no vkQueueWaitIdle (semaphore-synced present).
    let presentDirectImage configuration (vk: Vk) (swapchainExt: KhrSwapchain) device (swapchainState: SwapchainState) (skiaState: SkiaState) queueFamily (image: Image) imageIndex colorType scene =
        try
            let direct = swapchainState.Direct
            let width = int swapchainState.Extent.Width
            let height = int swapchainState.Extent.Height
            let imageInfo = swapchainImageInfo swapchainState queueFamily image

            use rt = new GRBackendRenderTarget(width, height, 1, imageInfo)
            use surface = SKSurface.Create(skiaState.Context, rt, GRSurfaceOrigin.TopLeft, colorType)

            if isNull surface then
                Result.Error(Diagnostics.create Error VulkanSwapchain "SkiaSharp did not wrap the swapchain image for direct present." None)
            else
                let clear =
                    configuration.ClearColor
                    |> Option.defaultValue Colors.black
                    |> SceneRenderer.skColor

                surface.Canvas.Clear clear
                drawScene scene surface.Canvas
                surface.Canvas.Flush()
                surface.Flush()
                skiaState.Context.Flush()
                // Submit Skia's render work to the queue (no CPU stall) so the transition
                // submitted next on the same queue orders after it.
                skiaState.Context.Submit(false)

                let mutable cmdBuf = direct.TransitionBuffers[int imageIndex]
                let mutable signalSem = direct.PresentSemaphores[int imageIndex]
                let mutable submitInfo = SubmitInfo()
                submitInfo.SType <- StructureType.SubmitInfo
                submitInfo.CommandBufferCount <- 1u
                submitInfo.PCommandBuffers <- &&cmdBuf
                submitInfo.SignalSemaphoreCount <- 1u
                submitInfo.PSignalSemaphores <- &&signalSem

                match checkResult VulkanSwapchain "vkQueueSubmit(direct present transition)" (vk.QueueSubmit(skiaState.Queue, 1u, &submitInfo, Fence(Nullable<uint64>()))) with
                | Result.Error diagnostic -> Result.Error diagnostic
                | Ok() ->
                    let mutable waitSem = signalSem
                    let mutable presentedSwapchain = swapchainState.Swapchain
                    let mutable presentImageIndex = imageIndex
                    let mutable presentInfo = PresentInfoKHR()
                    presentInfo.SType <- StructureType.PresentInfoKhr
                    presentInfo.WaitSemaphoreCount <- 1u
                    presentInfo.PWaitSemaphores <- &&waitSem
                    presentInfo.SwapchainCount <- 1u
                    presentInfo.PSwapchains <- &&presentedSwapchain
                    presentInfo.PImageIndices <- &&presentImageIndex

                    match checkResult VulkanSwapchain "vkQueuePresentKHR(direct present)" (swapchainExt.QueuePresent(skiaState.Queue, &presentInfo)) with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() ->
                        Ok
                            { Width = width
                              Height = height
                              ColorType = colorType
                              // No readback in direct mode; on-demand capture renders its own
                              // offscreen surface (FR-004). Empty pixels signal "render on demand".
                              Pixels = [||] }
        with ex ->
            Result.Error(Diagnostics.create Error VulkanSwapchain "Direct-to-swapchain present failed." (Some ex.Message))

    let renderFrameDirect configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) (skiaState: SkiaState) queueFamily (report: RenderDiagnostic -> unit) scene =
        let direct = swapchainState.Direct
        let fallback () =
            renderFrameReadback configuration vk swapchainExt physicalDevice device swapchainState skiaState queueFamily scene

        try
            match getSwapchainImages swapchainExt device swapchainState.Swapchain with
            | Result.Error diagnostic -> Result.Error diagnostic
            | Ok images ->
                let colorType = colorTypeForFormat swapchainState.Format

                // Lazy one-time init of the persistent direct resources for this swapchain.
                // initDirectPresent probes wrap capability first; on this SkiaSharp build the
                // probe fails (mono/SkiaSharp #1502) so Available stays false and every frame
                // degrades to the proven readback path (FR-005), announced once via `report`.
                if not direct.Attempted then
                    direct.Attempted <- true

                    match initDirectPresent configuration vk skiaState device queueFamily swapchainState images colorType with
                    | Ok() -> direct.Available <- true
                    | Result.Error diagnostic ->
                        direct.Available <- false
                        // FR-005: safe degradation with the actionable cause, then readback.
                        report diagnostic

                if not direct.Available then
                    fallback ()
                else
                    match createFence vk device with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok fence ->
                        try
                            match acquireImage vk swapchainExt device swapchainState.Swapchain fence with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok imageIndex ->
                                let image = images[int imageIndex]

                                match presentDirectImage configuration vk swapchainExt device swapchainState skiaState queueFamily image imageIndex colorType scene with
                                | Ok snapshot ->
                                    // FR-007: announce the live present mode once (Category =
                                    // Swapchain via the Stage→Category mapping), non-golden.
                                    if not direct.Announced then
                                        direct.Announced <- true
                                        report (Diagnostics.create Info VulkanSwapchain "present-mode=DirectToSwapchain readback=false (live frames render straight onto the swapchain image)." None)

                                    Ok snapshot
                                | Result.Error diagnostic ->
                                    // A per-frame direct failure after init: degrade to readback
                                    // from this frame onward (FR-005), skip presenting this frame.
                                    direct.Available <- false
                                    report (Diagnostics.create Warning VulkanSwapchain "DirectToSwapchain present failed mid-frame; falling back to the offscreen readback present path." diagnostic.Cause)
                                    Result.Error diagnostic
                        finally
                            if fence.Handle <> 0UL then
                                vk.DestroyFence(device, fence, nullPtr<AllocationCallbacks>)
        with ex ->
            Result.Error(Diagnostics.frameRenderFailed ex.Message)

    // Dispatch on the configured present mode. OffscreenReadback is byte-identical to the
    // pre-feature baseline; DirectToSwapchain opts into the readback-free path with a safe
    // fallback. `report` carries live-only, non-golden present diagnostics (FR-005/FR-007).
    let renderFrame configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) (skiaState: SkiaState) queueFamily (report: RenderDiagnostic -> unit) scene =
        match configuration.PresentMode with
        | ViewerPresentMode.OffscreenReadback ->
            renderFrameReadback configuration vk swapchainExt physicalDevice device swapchainState skiaState queueFamily scene
        | ViewerPresentMode.DirectToSwapchain ->
            renderFrameDirect configuration vk swapchainExt physicalDevice device swapchainState skiaState queueFamily report scene

    let encodeSnapshot (request: ScreenshotRequest) snapshot =
        try
            let directory = Path.GetDirectoryName request.Destination

            if not (String.IsNullOrWhiteSpace directory) then
                Directory.CreateDirectory directory |> ignore

            let imageInfo =
                SKImageInfo(snapshot.Width, snapshot.Height, snapshot.ColorType, SKAlphaType.Premul)

            let handle = GCHandle.Alloc(snapshot.Pixels, GCHandleType.Pinned)

            try
                use pixmap =
                    new SKPixmap(imageInfo, handle.AddrOfPinnedObject(), imageInfo.RowBytes)

                use image = SKImage.FromPixels pixmap

                if isNull image then
                    Result.Error(Diagnostics.screenshotFailed "SkiaSharp could not create an image from the last rendered Vulkan frame.")
                else
                    let format =
                        match request.Format with
                        | Png -> SKEncodedImageFormat.Png
                        | Jpeg -> SKEncodedImageFormat.Jpeg

                    use data = image.Encode(format, 90)

                    if isNull data then
                        Result.Error(Diagnostics.screenshotFailed "SkiaSharp could not encode the screenshot image.")
                    else
                        use stream = File.Open(request.Destination, FileMode.Create, FileAccess.Write, FileShare.None)
                        data.SaveTo stream
                        Ok()
            finally
                handle.Free()
        with ex ->
            Result.Error(Diagnostics.screenshotFailed ex.Message)

    let run program =
        let vk = Vk.GetApi()
        let mutable currentModel = Unchecked.defaultof<_>
        let mutable instance = Instance()
        let mutable surface = SurfaceKHR()
        let mutable device = Device()
        let mutable swapchainState: SwapchainState option = None
        let mutable skiaContext: GRContext option = None
        let mutable skiaExtensions: GRVkExtensions option = None
        let mutable window: IWindow option = None
        let mutable windowEventMapping: IDisposable option = None
        let mutable inputEventMapping: IDisposable option = None
        let mutable activeSubscriptions: IDisposable list = []
        let mutable pendingScene: Scene option = None
        let mutable pendingScreenshots: ScreenshotRequest list = []
        let mutable renderScene: (Scene -> Result<FrameSnapshot, RenderDiagnostic>) option = None
        // Feature 118 (FR-004): on-demand offscreen-readback capture, decoupled from per-frame
        // present, so screenshots/evidence work under both present modes (the direct present
        // path performs no readback). `lastScene` is the most recent rendered scene to re-render.
        let mutable captureScene: (Scene -> Result<FrameSnapshot, RenderDiagnostic>) option = None
        let mutable lastScene: Scene option = None
        let mutable lastFrame: FrameSnapshot option = None
        let mutable surfaceExt: KhrSurface option = None
        let mutable swapchainExt: KhrSwapchain option = None
        let mutable shutdownRequested = false

        let requestShutdown closeWindow =
            shutdownRequested <- true

            match window with
            | Some w ->
                if closeWindow && not w.IsClosing then
                    try
                        w.Close()
                    with _ ->
                        ()

                try
                    w.IsClosing <- true
                with _ ->
                    ()
            | None -> ()

        let disposeSubscriptions () =
            activeSubscriptions
            |> List.iter (fun subscription -> subscription.Dispose())

            activeSubscriptions <- []

        let rec saveScreenshot request snapshot =
            match encodeSnapshot request snapshot with
            | Ok() -> Ok()
            | Result.Error diagnostic ->
                dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                Result.Error diagnostic

        and flushPendingScreenshots snapshot =
            let requests = pendingScreenshots
            pendingScreenshots <- []

            requests
            |> List.fold
                (fun state request ->
                    match state with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() -> saveScreenshot request snapshot)
                (Ok())

        and interpretEffect effect =
            match effect with
            | InitializeRenderer -> Ok()
            | RenderFrame scene ->
                match renderScene with
                | Some render ->
                    lastScene <- Some scene

                    match render scene with
                    | Ok snapshot ->
                        lastFrame <- Some snapshot

                        // Direct present yields no readback pixels; flush any deferred captures
                        // by rendering the scene on demand through the offscreen routine (FR-004).
                        if snapshot.Pixels.Length > 0 then
                            flushPendingScreenshots snapshot
                        else
                            match captureScene with
                            | Some capture when not (List.isEmpty pendingScreenshots) ->
                                match capture scene with
                                | Ok captureSnapshot -> flushPendingScreenshots captureSnapshot
                                | Result.Error diagnostic ->
                                    dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                                    Result.Error diagnostic
                            | _ -> Ok()
                    | Result.Error diagnostic ->
                        dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                        Result.Error diagnostic
                | None ->
                    pendingScene <- Some scene
                    Ok()
            | CaptureScreenshot request ->
                // On-demand capture (FR-004): prefer the readback pixels already captured by the
                // offscreen present path; otherwise (direct present mode) render the last scene on
                // demand through the offscreen readback routine — never gated on the present mode.
                let captureOnDemand () =
                    match lastScene |> Option.orElse pendingScene, captureScene with
                    | Some scene, Some capture ->
                        match capture scene with
                        | Ok snapshot ->
                            lastFrame <- Some snapshot
                            saveScreenshot request snapshot
                        | Result.Error diagnostic ->
                            dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                            Result.Error diagnostic
                    | _ ->
                        let diagnostic =
                            Diagnostics.screenshotFailed "Screenshot capture was requested before the first successful Vulkan/Skia frame."

                        dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                        Result.Error diagnostic

                match lastFrame with
                | Some snapshot when snapshot.Pixels.Length > 0 -> saveScreenshot request snapshot
                | _ ->
                    match pendingScene with
                    | Some _ when Option.isNone captureScene ->
                        pendingScreenshots <- pendingScreenshots @ [ request ]
                        Ok()
                    | _ -> captureOnDemand ()
            | Shutdown ->
                requestShutdown true
                disposeSubscriptions ()
                Ok()
            | ReportDiagnostic diagnostic ->
                if program.Configuration.Diagnostics.Verbose then
                    Console.Error.WriteLine($"FS.Skia.UI diagnostic: {diagnostic.Stage}: {diagnostic.Message}")

                Ok()
            | Dispatch msg ->
                dispatch msg
                Ok()

        and dispatch msg =
            match program.EffectMapper msg with
            | Some effect ->
                interpretEffect effect |> ignore
            | None ->
                let nextModel, cmd = program.Update msg currentModel
                currentModel <- nextModel

                cmd
                |> List.iter (fun effect -> effect dispatch)

        let startSubscriptions () =
            disposeSubscriptions ()

            activeSubscriptions <-
                program.Subscriptions currentModel
                |> List.map (fun (_, subscribe) -> subscribe dispatch)

        let runEventLoop (createdWindow: IWindow) =
            if not shutdownRequested then
                trace program.Configuration "entering Silk.NET event loop"
                let frameInterval =
                    program.Configuration.TargetFrameRate
                    |> Option.defaultValue 60
                    |> max 1
                    |> fun frameRate -> 1.0 / float frameRate

                let stopwatch = System.Diagnostics.Stopwatch.StartNew()
                let mutable lastFrameTime = stopwatch.Elapsed.TotalSeconds

                while not createdWindow.IsClosing && not shutdownRequested do
                    createdWindow.DoEvents()

                    if shutdownRequested then
                        try
                            createdWindow.IsClosing <- true
                        with _ ->
                            ()
                    else
                        let now = stopwatch.Elapsed.TotalSeconds

                        if now - lastFrameTime >= frameInterval then
                            lastFrameTime <- now
                            createdWindow.DoUpdate()

                        if not shutdownRequested && not createdWindow.IsClosing then
                            createdWindow.DoRender()

                        Threading.Thread.Sleep(1)

        let execute () =
            try
                let initialModel, initialCmd = program.Init()
                currentModel <- initialModel
                initialCmd |> List.iter (fun effect -> effect dispatch)
                startSubscriptions ()

                result {
                    trace program.Configuration "creating Silk.NET window"
                    let! createdWindow = createWindow program.Configuration
                    window <- Some createdWindow
                    windowEventMapping <- Some(attachWindowEventMapping program createdWindow (fun () -> requestShutdown false) dispatch)

                    trace program.Configuration "initializing Silk.NET window"
                    do! initializeWindow createdWindow

                    trace program.Configuration "attaching Silk.NET input event mapping"
                    let! inputMapping = attachInputEventMapping program createdWindow dispatch
                    inputEventMapping <- Some inputMapping

                    trace program.Configuration "querying Vulkan surface source"
                    let! surfaceSource = getSurfaceSource createdWindow

                    trace program.Configuration "querying required Vulkan extensions"
                    let! (extensionNames, extensionCount) = copyRequiredExtensions surfaceSource

                    trace program.Configuration $"creating Vulkan instance extensionCount={extensionCount}"
                    let! createdInstance = createInstance vk extensionNames extensionCount
                    instance <- createdInstance

                    let createdSurfaceExt = new KhrSurface(vk.Context)
                    let createdSwapchainExt = new KhrSwapchain(vk.Context)
                    surfaceExt <- Some createdSurfaceExt
                    swapchainExt <- Some createdSwapchainExt

                    trace program.Configuration "creating Vulkan presentation surface"
                    let! createdSurface = createSurface surfaceSource instance
                    surface <- createdSurface

                    trace program.Configuration "enumerating physical devices"
                    let! physicalDevices = enumeratePhysicalDevices vk instance

                    trace program.Configuration $"choosing physical device count={physicalDevices.Length}"
                    let! (physicalDevice, queueFamily) = choosePhysicalDevice vk createdSurfaceExt surface physicalDevices

                    trace program.Configuration $"creating logical device physicalDevice={physicalDevice.Handle} queueFamily={queueFamily}"
                    let! createdDevice = createDevice vk physicalDevice queueFamily
                    device <- createdDevice

                    trace program.Configuration "creating swapchain"
                    let! createdSwapchain = createSwapchain program.Configuration createdSurfaceExt createdSwapchainExt physicalDevice device surface
                    swapchainState <- Some createdSwapchain

                    let! skia = createSkiaContext program.Configuration vk instance physicalDevice device queueFamily
                    skiaContext <- Some skia.Context
                    skiaExtensions <- Some skia.Extensions

                    // Live-only, non-golden present diagnostics (FR-005 fallback Warning,
                    // FR-007 present-mode Info) flow over the existing diagnostic channel.
                    let report diagnostic =
                        dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)

                    renderScene <-
                        Some(renderFrame program.Configuration vk createdSwapchainExt physicalDevice device createdSwapchain skia queueFamily report)

                    // On-demand offscreen-readback capture routine (FR-004), independent of the
                    // present mode — the same readback Skia surface the offscreen path renders.
                    let captureColorType = colorTypeForFormat createdSwapchain.Format

                    captureScene <-
                        Some(fun scene ->
                            bind (renderSceneToPixels program.Configuration skia createdSwapchain.Extent captureColorType scene) (fun pixels ->
                                Ok
                                    { Width = int createdSwapchain.Extent.Width
                                      Height = int createdSwapchain.Extent.Height
                                      ColorType = captureColorType
                                      Pixels = pixels }))

                    let scene =
                        pendingScene
                        |> Option.defaultValue (program.View currentModel)

                    pendingScene <- None
                    do! interpretEffect (RenderFrame scene)
                    runEventLoop createdWindow
                    return ()
                }
            with ex ->
                Result.Error(Diagnostics.frameRenderFailed ex.Message)

        try
            execute ()
        finally
            disposeSubscriptions ()

            match inputEventMapping with
            | Some mapping -> mapping.Dispose()
            | None -> ()

            match windowEventMapping with
            | Some mapping -> mapping.Dispose()
            | None -> ()

            match skiaContext with
            | Some context -> context.Dispose()
            | None -> ()

            match skiaExtensions with
            | Some extensions -> extensions.Dispose()
            | None -> ()

            match swapchainExt with
            | Some ext ->
                match swapchainState with
                | Some state when state.Swapchain.Handle <> 0UL ->
                    let _ = vk.DeviceWaitIdle(device)
                    // Feature 118: release the persistent direct-present resources (command
                    // pool + per-image present semaphores) before destroying the swapchain.
                    disposeDirectPresent vk device state.Direct
                    ext.DestroySwapchain(device, state.Swapchain, nullPtr<AllocationCallbacks>)
                | _ -> ()
            | _ -> ()

            if device.Handle <> IntPtr.Zero then
                vk.DestroyDevice(device, nullPtr<AllocationCallbacks>)

            match surfaceExt with
            | Some ext when surface.Handle <> 0UL -> ext.DestroySurface(instance, surface, nullPtr<AllocationCallbacks>)
            | _ -> ()

            if instance.Handle <> IntPtr.Zero then
                vk.DestroyInstance(instance, nullPtr<AllocationCallbacks>)

            match window with
            | Some w ->
                try
                    w.IsClosing <- true
                with _ ->
                    ()

                w.Dispose()
            | None -> ()

