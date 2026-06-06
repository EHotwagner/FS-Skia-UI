// CONTRACT SKETCH (Phase 1) — stateful façades. Each REUSES an existing MVU model
// (no fork — FR-004/SC-003) and delegates `init`/`update` to it. Shape mirrors the
// 065 TextBoxWidget. text-area → TextInput; the selection collections → Collections.
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

// ---- text-area: reuses TextInputModel/Msg/Effect (exactly like 065 TextBox) ----
type TextAreaProps<'msg> =
    { Id: ControlId
      Value: string
      ReadOnly: bool
      Validation: ValidationState
      OnChanged: (string -> 'msg) option }

module TextArea =
    val defaults: controlId: ControlId -> TextAreaProps<'msg>
    /// Delegates to `TextInput.init` (multiline) — model + effects equal the existing control.
    val init: props: TextAreaProps<'msg> -> TextInputModel * TextInputEffect list
    /// Delegates to `TextInput.update` — pure, no I/O (result equality asserted, SC-003).
    val update: msg: TextInputMsg -> model: TextInputModel -> TextInputModel * TextInputEffect list
    /// Lowers ≡ legacy `TextArea.create` for the current model state.
    val view: props: TextAreaProps<'msg> -> model: TextInputModel -> Widget<'msg>

// ---- selection collections: five per-id modules over the SAME Collections model ----
type ListViewProps<'msg> =
    { Id: ControlId; Items: ListItem list; SelectedKey: string option; OnSelected: (string -> 'msg) option }
type ListBoxProps<'msg> =
    { Id: ControlId; Items: ListItem list; SelectedKey: string option; OnSelected: (string -> 'msg) option }
type MultiSelectListProps<'msg> =
    { Id: ControlId; Items: ListItem list; SelectedKeys: Set<string>; OnChanged: (string list -> 'msg) option }
type ComboBoxProps<'msg> =
    { Id: ControlId; Items: ListItem list; SelectedKey: string option; IsOpen: bool; OnChanged: (string -> 'msg) option }
type TreeViewProps<'msg> =
    { Id: ControlId; Items: TreeNode list; Expanded: Set<string>; OnSelected: (string -> 'msg) option }

module ListView =
    val defaults: controlId: ControlId -> ListViewProps<'msg>
    val init: props: ListViewProps<'msg> -> CollectionModel * CollectionEffect list   // delegates to Collections.init
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list  // delegates to Collections.update
    /// Lowers ≡ legacy `Control.standard <list-view kind>` for the current model state.
    val view: props: ListViewProps<'msg> -> model: CollectionModel -> Widget<'msg>

module ListBox =
    val defaults: controlId: ControlId -> ListBoxProps<'msg>
    val init: props: ListBoxProps<'msg> -> CollectionModel * CollectionEffect list
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
    val view: props: ListBoxProps<'msg> -> model: CollectionModel -> Widget<'msg>

module MultiSelectList =
    val defaults: controlId: ControlId -> MultiSelectListProps<'msg>
    val init: props: MultiSelectListProps<'msg> -> CollectionModel * CollectionEffect list
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
    val view: props: MultiSelectListProps<'msg> -> model: CollectionModel -> Widget<'msg>

module ComboBox =
    val defaults: controlId: ControlId -> ComboBoxProps<'msg>
    val init: props: ComboBoxProps<'msg> -> CollectionModel * CollectionEffect list
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
    val view: props: ComboBoxProps<'msg> -> model: CollectionModel -> Widget<'msg>

module TreeView =
    val defaults: controlId: ControlId -> TreeViewProps<'msg>
    val init: props: TreeViewProps<'msg> -> CollectionModel * CollectionEffect list
    val update: msg: CollectionMsg -> model: CollectionModel -> CollectionModel * CollectionEffect list
    val view: props: TreeViewProps<'msg> -> model: CollectionModel -> Widget<'msg>
