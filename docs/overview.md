# CSCS_WPF Web Overview

## Purpose
CSCS_WPF supports development of web-oriented applications and integrations in addition to desktop WPF applications.  
The web feature set includes:
- HTTP endpoint creation
- HTTP response generation
- JSON serialization and deserialization
- application configuration lookup
- HTML template loading and rendering
- form value extraction
- outbound web requests to external services [file:104]

## Core concepts

### Inbound web flow
A typical inbound request flow is:
1. open an endpoint with `CreateEndpoint(method, route, handlerFunctionName)`
2. receive a request on that route
3. execute the specified handler function
4. return one of:
   - no value / empty result
   - plain string
   - structured response object [file:104]

### Outbound web flow
A typical outbound integration flow is:
1. prepare payload as string
2. optionally prepare headers dictionary
3. call `WebRequest(...)` or `WebRequestMPFD(...)`
4. handle success or failure in callback functions
5. optionally wait synchronously for result if `justFire = false` [file:104]

### JSON support
CSCS_WPF supports:
- `SerializeJson(object)` to convert hash-tables or arrays into JSON
- `DeserializeJson(jsonString)` to parse JSON into array or hash-table
- `Sql2Json(sqlResult2D)` to transform SQL query output into JSON string [file:104]

### HTML template support
CSCS_WPF supports server-side HTML rendering with:
- `LoadTemplate(path)`
- `FillTemplateFromDictionary(handle, values)`
- `FillTemplatePlaceholder(handle, placeholderName, value)`
- `RenderCondition(handle, conditionName, bool)`
- `RenderHtml(handle)` [file:104]

## Main web functions
| Function | Purpose |
|---|---|
| `CreateEndpoint(...)` | Opens an HTTP route and binds it to a handler function |
| `Response(...)` | Creates a structured HTTP response |
| `ReadConfig(...)` | Reads a value from configuration |
| `SerializeJson(...)` | Converts array/hash-table to JSON string |
| `DeserializeJson(...)` | Converts JSON string to array/hash-table |
| `Sql2Json(...)` | Converts 2D SQL result to JSON string |
| `LoadTemplate(...)` | Loads HTML template into memory |
| `FillTemplateFromDictionary(...)` | Replaces multiple template placeholders |
| `FillTemplatePlaceholder(...)` | Replaces one template placeholder |
| `RenderCondition(...)` | Keeps or removes a template block |
| `RenderHtml(...)` | Produces final HTML string |
| `GetValueFromForm(...)` | Extracts one value from form body string |
| `WebRequest(...)` | Sends HTTP request to external service |
| `WebRequestMPFD(...)` | Sends multipart/form-data request to external service | [file:104]

## Recommended structure for AI-generated web code
When generating CSCS_WPF web code, prefer this structure:
- declare endpoint
- declare handler function
- prepare response body
- serialize JSON if needed
- return `Response(...)` when headers or custom status code are needed [file:104]

## Minimal example
```internal-script
CreateEndpoint("GET", "/", "processGETroot");

function processGETroot
{
    headers = {};
    headers["Content-Type"] = "text/plain";
    return Response(headers, "Hello world", 200);
}
```

## Use when
Use the web feature set when:
- exposing API endpoints
- returning JSON data
- rendering HTML from templates
- receiving submitted form data
- calling external REST services
- sending multipart requests with files [file:104]

## AI notes
- Do not mix WPF window events with web endpoint handlers.
- `CreateEndpoint(...)` is the entry point for inbound web processing.
- Use `Response(...)` when custom headers or status codes are required.
- Prefer `SerializeJson(...)` over manual JSON string concatenation.
- Prefer template functions over manual HTML string assembly when template rendering is intended. [file:104]