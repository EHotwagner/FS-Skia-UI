namespace FS.Skia.UI.Controls

module Attr =
    let create name category value =
        { Name = name
          Category = category
          Value = value }

    let text value = create "text" Content (TextValue value)
    let value value = create "value" Content (TextValue value)
    let items values = create "items" Data (StringListValue values)
    let child control = create "child" Children (ChildValue control)
    let children controls = create "children" Children (ChildrenValue controls)
    let enabled value = create "enabled" State (BoolValue value)
    let visible value = create "visible" State (BoolValue value)
    let readOnly value = create "readOnly" State (BoolValue value)
    let loading value = create "loading" State (BoolValue value)
    let selected value = create "selected" State (BoolValue value)
    let width value = create "width" Layout (FloatValue value)
    let height value = create "height" Layout (FloatValue value)
    let padding value = create "padding" Layout (FloatValue value)
    let margin value = create "margin" Layout (FloatValue value)
    let style name = create "style" Style (TextValue name)
    let theme theme = create "theme" Theme (ThemeValue theme)
    let validation state = create "validation" Validation (ValidationValue state)
    let accessibility metadata = create "accessibility" Accessibility (AccessibilityValue metadata)
    let on eventKind msg = create eventKind Event (MessageValue msg)
    let onWith eventKind map = create eventKind Event (EventValue map)
