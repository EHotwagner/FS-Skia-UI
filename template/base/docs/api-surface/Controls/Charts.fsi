namespace FS.Skia.UI.Controls

// `ChartPoint` / `ChartSeries` are declared in Types.fsi (feature 080, surface-neutral move).

/// Public contract module exposed by this FS.Skia.UI package.
module LineChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module BarChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module PieChart =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val values: ChartPoint list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module ScatterPlot =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val series: ChartSeries list -> Attr<'msg>

/// Public contract module exposed by this FS.Skia.UI package.
module GraphView =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val nodes: string list -> Attr<'msg>
