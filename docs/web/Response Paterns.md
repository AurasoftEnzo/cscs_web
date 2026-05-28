---
title: Response Patterns
module: web
topic: response
applies_to: CSCS_WEB
version: 1
---

# Response Patterns

## Purpose
`Response(...)` creates a structured HTTP response for a CSCS_WPF endpoint.  
Use it when the handler must explicitly control headers, body and status code.

## Syntax
```internal-script
response = Response(headers, body, statusCode);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `headers` | hash-table / dictionary | yes | Response headers |
| `body` | string | yes | Response body |
| `statusCode` | integer | yes | HTTP status code | 

## Returns
Returns a response object that can be returned from an endpoint handler .

## Why use Response(...)
Use `Response(...)` when:
- the endpoint must return JSON with correct content type
- the endpoint must return HTML
- custom headers are needed
- CORS headers are needed
- non-default HTTP status code is needed 

## Common response patterns

### Plain text
```internal-script
function getPing
{
    headers = {};
    headers["Content-Type"] = "text/plain";
    return Response(headers, "pong", 200);
}
```

### JSON
```internal-script
function getStatus
{
    payload = {};
    payload["status"] = "ok";
    payload["version"] = "1.0";

    json = SerializeJson(payload);

    headers = {};
    headers["Access-Control-Allow-Origin"] = "*";
    headers["Content-Type"] = "application/json";

    return Response(headers, json, 200);
}
```

### HTML
```internal-script
function homePage
{
    html = "<h1>Hello world</h1>";

    headers = {};
    headers["Content-Type"] = "text/html";

    return Response(headers, html, 200);
}
```

### Error response
```internal-script
function invalidRequest
{
    payload = {};
    payload["error"] = "invalid request";
    payload["code"] = 400;

    json = SerializeJson(payload);

    headers = {};
    headers["Content-Type"] = "application/json";

    return Response(headers, json, 400);
}
```

## Header examples
Typical headers include:
- `Content-Type`
- `Access-Control-Allow-Origin`
- `Cache-Control`
- custom application headers 

Example:
```internal-script
headers = {};
headers["Access-Control-Allow-Origin"] = "*";
headers["Content-Type"] = "application/json";
headers["Cache-Control"] = "no-cache";
```

## Valid and invalid usage

### Valid
```internal-script
headers = {};
headers["Content-Type"] = "application/json";
return Response(headers, jsonString, 200);
```

### Invalid
```internal-script
return Response("Access-Control-Allow-Origin, Content-Type=application/json", jsonString, 200);
```

Reason:
`headers` must be a dictionary / hash-table, not a flat string.

## Best practices
- Always set `Content-Type` explicitly for JSON and HTML responses.
- Use `SerializeJson(...)` before returning JSON body.
- Build the `headers` dictionary first, then call `Response(...)`.
- Use consistent error body structure for API endpoints. 

## AI notes
- `Response(...)` expects a dictionary as the first argument.
- `body` should be a string.
- `statusCode` should be an integer.
- Do not invent shorthand header formats.
- For APIs, explicit response construction is preferable to implicit string returns.