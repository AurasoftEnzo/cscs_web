---
title: CreateEndpoint
module: web
topic: endpoint
applies_to: CSCS_WEB
version: 1
---

# CreateEndpoint

## Purpose
`CreateEndpoint(...)` opens an HTTP endpoint on a specified route and binds it to a handler function.  
When the route is called with the matching HTTP method, the specified handler function is executed.

## Syntax
```internal-script
CreateEndpoint(httpMethod, route, handlerFunctionName);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `httpMethod` | string | yes | HTTP method, such as `GET`, `POST`, `PUT`, `DELETE` |
| `route` | string | yes | Route on which the endpoint will be accessible |
| `handlerFunctionName` | string | yes | Name of the function that handles the request | 

## Supported methods
The manual explicitly describes these methods:
- `GET`
- `POST`
- `PUT`
- `DELETE` 

## Handler behavior
The handler function is called when the endpoint receives a request matching the declared method and route.  
The handler can return different shapes of values, and the runtime converts them into an HTTP response.

## Return modes

### 1. Return no content
If the handler returns no meaningful value, the runtime treats it as empty response and returns `204 No Content`.

Example:
```internal-script
CreateEndpoint("GET", "/", "processGETroot");

function processGETroot
{
    return;
}
```

### 2. Return plain string
If the handler returns a string, the runtime automatically:
- uses that string as response body
- sets content type to `text/plain`
- sets status code to `200`

Example:
```internal-script
CreateEndpoint("GET", "/", "processGETroot");

function processGETroot
{
    return "some plain text";
}
```

### 3. Return structured response
If the handler returns a structured response object or `Response(...)`, the runtime uses:
- `headers`
- `body`
- `statusCode`

Example:
```internal-script
CreateEndpoint("GET", "/", "processGETroot");

function processGETroot
{
    headers = {};
    headers["Content-Type"] = "text/html";
    body = "<h1>Hello world</h1>";
    statusCode = 200;
    return Response(headers, body, statusCode);
}
```

## Valid examples

### Simple GET endpoint
```internal-script
CreateEndpoint("GET", "/", "processGETroot");

function processGETroot
{
    return "Hello from root";
}
```

### JSON endpoint
```internal-script
CreateEndpoint("GET", "/api/status", "getStatus");

function getStatus
{
    payload = {};
    payload["status"] = "ok";
    payload["message"] = "service is running";

    json = SerializeJson(payload);

    headers = {};
    headers["Access-Control-Allow-Origin"] = "*";
    headers["Content-Type"] = "application/json";

    return Response(headers, json, 200);
}
```

### HTML endpoint
```internal-script
CreateEndpoint("GET", "/home", "renderHome");

function renderHome
{
    headers = {};
    headers["Content-Type"] = "text/html";
    return Response(headers, "<h1>Home</h1>", 200);
}
```

## Common mistakes
- Passing handler code instead of handler function name.
- Returning a custom structure that does not contain the expected response fields.
- Returning JSON text as plain string without setting `Content-Type` when API clients expect `application/json`.
- Mixing endpoint handler logic with WPF event handler patterns.

## Best practices
- Keep one endpoint declaration per route.
- Use descriptive handler names such as `getCustomers`, `createInvoice`, `renderHomePage`.
- Use `Response(...)` whenever headers or status code must be controlled explicitly.
- For JSON APIs, serialize the payload and set `Content-Type` to `application/json`. 

## AI notes
- The third argument is the handler function name, not arbitrary code.
- A returned plain string is not the same as a JSON response with explicit headers.
- When in doubt, prefer explicit `Response(...)` over implicit string return.
- Do not invent unsupported return conventions. Stick to:
  - empty return
  - plain string
  - structured response via `Response(...)` 