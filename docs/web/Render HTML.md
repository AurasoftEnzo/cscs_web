---
title: RenderHtml
module: web
topic: templates
applies_to: CSCS_WPF
version: 1
---

# RenderHtml

## Purpose
`RenderHtml(templateHndl)` renders a previously loaded and prepared template into final HTML string.

This is the last step of the template workflow, after:
- `LoadTemplate(...)`
- `FillTemplateFromDictionary(...)` and/or `FillTemplatePlaceholder(...)`
- optional `RenderCondition(...)` 

## Syntax
```internal-script
htmlString = RenderHtml(templateHndl);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `templateHndl` | integer | yes | Handle of previously loaded template | [file:104]

## Returns
Returns rendered HTML string.

## Basic example
```internal-script
template1hndl = LoadTemplate("C:\\templates\\page1.html");
html1string = RenderHtml(template1hndl);
```

## Standard workflow
Typical HTML rendering flow:
1. load template
2. fill placeholders
3. optionally process conditions
4. render HTML
5. return response with `Content-Type = text/html`

## Minimal endpoint example
```internal-script
CreateEndpoint("GET", "/home", "renderHome");

function renderHome
{
    templateHndl = LoadTemplate("C:\\templates\\home.html");

    values = {};
    values["pageTitle"] = "Home";
    values["welcomeText"] = "Welcome to CSCS_WPF web";

    FillTemplateFromDictionary(templateHndl, values);

    htmlString = RenderHtml(templateHndl);

    headers = {};
    headers["Content-Type"] = "text/html";

    return Response(headers, htmlString, 200);
}
```

## Example with conditional rendering
```internal-script
function renderDashboard
{
    templateHndl = LoadTemplate("C:\\templates\\dashboard.html");

    values = {};
    values["userName"] = "Admin";
    values["todayDate"] = "2026-05-28";

    FillTemplateFromDictionary(templateHndl, values);

    RenderCondition(templateHndl, "showAdminPanel", true);

    htmlString = RenderHtml(templateHndl);

    headers = {};
    headers["Content-Type"] = "text/html";

    return Response(headers, htmlString, 200);
}
```

## Relationship to Response(...)
`RenderHtml(...)` only produces HTML string.  
It does not send HTTP response by itself.  
To return the rendered page from endpoint handler, combine it with `Response(...)`.

## When to use
Use `RenderHtml(...)` when:
- template has already been loaded
- placeholder replacement is complete
- final HTML string must be returned to client
- template-driven server-side HTML page is needed

## Common mistakes
- Calling `RenderHtml(...)` before loading the template.
- Calling `RenderHtml(...)` before replacing placeholders.
- Treating rendered HTML as response object.
- Forgetting `Content-Type = text/html` when returning rendered HTML.

## Best practices
- Render only once the template is fully prepared.
- Keep rendering close to the return statement in endpoint handler.
- Use explicit HTML response headers.
- Keep template pipeline readable:
  `LoadTemplate -> Fill -> optional RenderCondition -> RenderHtml -> Response`
## AI notes
- `RenderHtml(...)` returns string, not response object.
- It should usually appear near the end of the handler.
- If the task is �return HTML page�, combine rendered string with `Response(...)`.
- Do not skip placeholder fill step if the page depends on dynamic values.