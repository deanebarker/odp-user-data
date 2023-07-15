# Opti Data Profiles

Provides a simple C# wrapper around customer objects in Optimizely Data Plaform (ODP), allowing you to use that repository to store user profile and preferences.

## To Install

You only need to compile in the one class: `OptiDataProfile.cs`

Set two static properties:

```
// The name of the field that is serving as an identifier
OptiDataProfile.KeyField = "email";

// Your API Key
OptiDataProfile.ApiKey = "whatever";
```

## To Use (read)

Simple instantiate the object with the identifier. For example, if we were using email:

```
var profile = new OptiDataProfile("deane@deanebarker.net");
```

If a customer object doesn't exist in ODP for that key, it will be created.

The object is created in C#, all the profile data is cached in a local dictionary, under the `Attributes` property.

```
var address = profile.GetValue("address"); // Returns an object
var address = profile.GetString("address"); // Returns a string
var address = profile["address"]; // Returns a string
```

You can provide a default value to be used if `GetValue` returns `null` or `GetString` returns `null` or whitespace.

```
var mood = profile.GetString("mood", "Unknown");
```

## To Use (write)

You can write back to ODP one value at a time:

```
profile.SetValue("mood", "Happy");
```

That will write immediately, and update the local object cache.

You can write multiple keys at once:

```
profile.SetValues(new Dictionary<string,object>()
{
  { "mood", "Happy" },
  { "eyecolor", "blue" }
});
```

Or you can pass in an object that will be reflected:

```
profile.SetValues(new {
  mood = "Happy",
  eyecolor = "blue"
});
```

(Clearly, that last method presents issues with property naming. Your attribute keys would have to obey C# syntax rules -- no spaces, no hyphens, etc. If your keys violate those rules, then you'll need to use the `Dictionary` method.)

## Automating ID Retrieval

Normally, you need to pass in the ID to instatiate an object. This isn't ideal if you're instantiating the object in different locations in your code

You can set a static method to be used for this, which allows you to centralize this logic. This method should return a `string`.

```
OptiDataProfile.IdProvider = () => {
  var email = GetEmailSomehow();
  return email;
};
```

This method takes no parameters -- the intention is that you would retrieve the ID from `Session` or `HttpContext` or some other global construct.

Once this method is set, then anywhere in your code, you can call this:

```
var profile = OptiDataProfile.GetCurrentUser();
```

(See `ProfileController.cs` for examples of this method.)

That will call the `IdProvider` method, and feed the result into the constructor, returning a populated profile. If your `IdProvider` returns `null`, then `GetCurrentUser` will return `null` as well.
