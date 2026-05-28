---
title: LoadTemplate
module: web
topic: templates
applies_to: CSCS_WEB
version: 1
---

# LoadTemplate

## Purpose
`LoadTemplate(templatePath)` reads an HTML file into memory and returns a template handle.  
That handle is then used by:
- `FillTemplateFromDictionary(...)`
- `FillTemplatePlaceholder(...)`
- `RenderCondition(...)`
- `RenderHtml(...)` 

## Syntax
```internal-script
templateHndl = LoadTemplate(templatePath);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `templatePath` | string | yes | Path to the HTML template file | 

## Returns
Returns integer template handle .

## Template lifecycle
Typical usage flow:
1. load template using `LoadTemplate(...)`
2. replace placeholders
3. optionally apply `RenderCondition(...)`
4. produce final HTML with `RenderHtml(...)`
5. return HTML in `Response(...)` 

## Placeholder concept
The template may contain placeholders such as:
```html
{{customerName}}
```

These placeholders can later be replaced from script using:
- `FillTemplateFromDictionary(...)`
- `FillTemplatePlaceholder(...)` 

## Basic example
```internal-script
template1hndl = LoadTemplate("C:\\templates\\page1.html");
```

## Typical HTML page flow
```internal-script
function renderHome
{
    templateHndl = LoadTemplate("C:\\templates\\home.html");

    values = {};
    values["pageTitle"] = "Home";
    values["welcomeText"] = "Hello from CSCS_WPF";

    FillTemplateFromDictionary(templateHndl, values);

    htmlString = RenderHtml(templateHndl);

    headers = {};
    headers["Content-Type"] = "text/html";

    return Response(headers, htmlString, 200);
}
```

## When to use
Use `LoadTemplate(...)` when:
- HTML output should come from template file instead of string concatenation
- multiple placeholders must be populated dynamically
- conditional content blocks are needed
- the same template is reused with different data 

## Common mistakes
- Calling `RenderHtml(...)` before loading a template.
- Passing invalid file path.
- Treating returned handle as HTML string.
- Expecting placeholders to be replaced automatically without calling fill functions .

## Best practices
- Load template first and keep the handle in a variable with clear name such as `templateHndl`.
- Use dictionary replacement when multiple placeholders are involved.
- Use single placeholder replacement only for isolated changes.
- Return rendered HTML through explicit `Response(...)` with `Content-Type = text/html`. 

## AI notes
- `LoadTemplate(...)` returns a handle, not rendered HTML.
- The handle is required in all later template functions.
- Do not replace placeholders by manual string concatenation if the template workflow is intended.
- If template rendering is used, the normal pipeline is:
  `LoadTemplate -> Fill... -> optional RenderCondition -> RenderHtml -> Response`. 