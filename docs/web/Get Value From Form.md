---
title: GetValueFromForm
module: web
topic: forms
applies_to: CSCS_WEB
version: 1
---

# GetValueFromForm

## Purpose
`GetValueFromForm(formString, key)` extracts a single value from a submitted form body string.  
This is useful in web endpoint handlers that receive classic form-encoded request content rather than JSON payload .

## Syntax
```internal-script
value = GetValueFromForm(formString, key);
```

## Arguments
| Argument | Type | Required | Description |
|---|---|---|---|
| `formString` | string | yes | Full form body string |
| `key` | string | yes | Name of the form item whose value should be retrieved | 

## Returns
Returns string value of the requested item .

## Basic example
```internal-script
body = requestBody;
surname = GetValueFromForm(body, "surname");
```

If the request body contains a form field named `surname`, the function returns its value .

## Example from manual pattern
```internal-script
body = requestBody;
name = GetValueFromForm(body, "name");
surname = GetValueFromForm(body, "surname");
```

## Typical POST handler example
```internal-script
CreateEndpoint("POST", "/submit-customer", "submitCustomer");

function submitCustomer
{
    body = requestBody;

    code = GetValueFromForm(body, "code");
    name = GetValueFromForm(body, "name");
    city = GetValueFromForm(body, "city");

    result = {};
    result["code"] = code;
    result["name"] = name;
    result["city"] = city;
    result["status"] = "received";

    json = SerializeJson(result);

    headers = {};
    headers["Content-Type"] = "application/json";

    return Response(headers, json, 200);
}
```

## HTML form example
```html
<form method="post" action="/submit-customer">
  <input type="text" name="code" />
  <input type="text" name="name" />
  <input type="text" name="city" />
  <button type="submit">Send</button>
</form>
```

## Validation example
```internal-script
function submitCustomer
{
    body = requestBody;

    code = GetValueFromForm(body, "code");
    name = GetValueFromForm(body, "name");

    if (code.EMPTYORWHITE || name.EMPTYORWHITE)
    {
        err = {};
        err["error"] = "Missing required fields";

        headers = {};
        headers["Content-Type"] = "application/json";

        return Response(headers, SerializeJson(err), 400);
    }

    ok = {};
    ok["status"] = "ok";
    ok["code"] = code;
    ok["name"] = name;

    headers = {};
    headers["Content-Type"] = "application/json";

    return Response(headers, SerializeJson(ok), 200);
}
```

## When to use
Use `GetValueFromForm(...)` when:
- request body is form data instead of JSON
- HTML `<form>` posts data to endpoint
- only one or a few keys must be extracted from body string
- simple form handling is enough without full JSON parsing 

## When not to use
Do not use `GetValueFromForm(...)` when:
- payload is JSON string, use `DeserializeJson(...)` instead
- the request is multipart/form-data with file upload logic
- request data already exists in structured object form 

## Common mistakes
- Trying to parse JSON body with `GetValueFromForm(...)`.
- Passing only one field value instead of the whole request body.
- Expecting non-string return type.
- Using wrong key names that do not match submitted form field names .

## Best practices
- Read all needed values from the same request body variable.
- Validate required values immediately after extraction.
- Convert response to JSON using `SerializeJson(...)` if endpoint is an API.
- Keep form field names and extraction keys identical. 

## AI notes
- `GetValueFromForm(...)` expects the full form body string and one key name.
- The return value is string.
- For JSON payloads, prefer `DeserializeJson(...)`, not `GetValueFromForm(...)`.
- Use this function mainly in classic HTML form POST handlers. 