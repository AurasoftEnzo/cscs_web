---
title: RenderCondition
module: web
topic: templates
applies_to: CSCS_WPF
version: 1
---

# RenderCondition

## Purpose
`RenderCondition(templateHndl, conditionName, condition)` controls whether a marked block of code in an HTML template remains in the final output or is removed.  
This is used for simple server-side conditional rendering in CSCS_WPF template workflows .

## Syntax
```internal-script
RenderCondition(templateHndl, conditionName, condition);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `templateHndl` | integer | yes | Handle of previously loaded template |
| `conditionName` | string | yes | Name of the condition used inside the template |
| `condition` | boolean | yes | Determines whether the marked block remains or is removed | 

## Returns
Returns `Variable.EmptyInstance` .

## Template markup rule
Conditional blocks in HTML template are wrapped using `IFconditionName` markers .

Example template:
```html
<html>
<body>
IFcondition1
<p>Lorem Ipsum</p>
IFcondition1
</body>
</html>
```

If `RenderCondition(templateHndl, "condition1", true)` is called, the block remains.  
If `RenderCondition(templateHndl, "condition1", false)` is called, the block is removed .

## Basic example
```internal-script
template1hndl = LoadTemplate("C:\\templates\\page1.html");
RenderCondition(template1hndl, "condition1", 1 < 2);
html1string = RenderHtml(template1hndl);
```

## Example with page section toggle
```internal-script
function renderDashboard
{
    templateHndl = LoadTemplate("C:\\templates\\dashboard.html");

    values = {};
    values["userName"] = "Admin";
    values["todayDate"] = "2026-05-28";

    FillTemplateFromDictionary(templateHndl, values);

    RenderCondition(templateHndl, "showAdminPanel", true);
    RenderCondition(templateHndl, "showWarningBox", false);

    htmlString = RenderHtml(templateHndl);

    headers = {};
    headers["Content-Type"] = "text/html";

    return Response(headers, htmlString, 200);
}
```

## Example HTML for multiple conditions
```html
<html>
<body>
<h1>{{userName}}</h1>

IFshowAdminPanel
<div class="admin-panel">Admin tools are enabled.</div>
IFshowAdminPanel

IFshowWarningBox
<div class="warning">There is an active warning.</div>
IFshowWarningBox

</body>
</html>
```

## When to use
Use `RenderCondition(...)` when:
- an HTML section should appear only under some condition
- the same template is reused for different roles or states
- a simple feature toggle is needed in rendered output
- conditional blocks are easier than preparing multiple template files 

## Relationship to other template functions
Typical sequence:
1. `LoadTemplate(...)`
2. `FillTemplateFromDictionary(...)` and/or `FillTemplatePlaceholder(...)`
3. `RenderCondition(...)`
4. `RenderHtml(...)`
5. return HTML via `Response(...)` 

## Common mistakes
- Calling `RenderCondition(...)` before `LoadTemplate(...)`.
- Using a `conditionName` that does not match the template markers.
- Expecting `RenderCondition(...)` to replace placeholders; it only keeps or removes a block.
- Calling `RenderHtml(...)` before all conditional blocks are processed .

## Best practices
- Use descriptive condition names such as `showAdminPanel`, `showCustomerData`, `showTotals`.
- Keep condition markers consistent across templates.
- Apply all conditions before final rendering.
- Use conditional rendering for structure, and placeholder replacement for values. 

## AI notes
- `RenderCondition(...)` does not return HTML; it modifies the loaded template state.
- The template must already be loaded.
- `conditionName` must match the `IFconditionName` markers in the template.
- Do not use `RenderCondition(...)` as a replacement mechanism for values; use fill functions for that. 