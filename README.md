# Opti Data Profiles

This library (just one class, really) provides a simple C# wrapper around customer objects in Optimizely Data Plaform (ODP), allowing you to use that repository to store user profile and preferences.

This allows storage of user preferences and data in ODP, which provides some benefits:

1. It's handy and simple
2. It scales like crazy (I'm not saying users in CMS _didn't_ scale, but ODP is literally built for exactly this)
3. You can segment users based on profile data
4. It's easy to re-use user data in other applications and platforms

Traditionally, Opti CMS hasn't provided a comprehensive framework for storing user data beyond name, email, and passwords. For other preference data, Opti CMS has relied on the ASP.NET profile framework or bespoke systems.

This library (hopefully) solves this issue.

## To Install

You only need to compile in the one class: `OptiDataProfile.cs`

Then, set two static properties:

```
// The name of the field that is serving as an identifier
OptiDataProfile.KeyField = "email";

// Your API Key
OptiDataProfile.ApiKey = "whatever";
```

(Yes, yes, I should probably write this as an injected service at some point. If you want to, go nuts. PRs are open.)

## To Use (read)

Instantiate the `OptiDataProfile` object with the identifier (a value for whatever you used as the `KeyField`). This should correspond to how the user is dentified in CMS -- either username or email would work.

For example, if we were using email:

```
var profile = new OptiDataProfile("deane@deanebarker.net");
```

If a customer object doesn't exist in ODP for that key, it will be created.

When the object is created in C#, all the profile data is cached in a local dictionary, under the `Attributes` property. Values are stored as `object`.

Some convenience accessors are also provided.

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

Note: that attribute must already exist on the "Customer" schema in ODP.

That will write immediately, and update the local object cache (it does this interally -- it doesn't write and then reload all the attributes).

You can write multiple keys at once:

```
profile.SetValues(new Dictionary<string,object>()
{
  { "mood", "Happy" },
  { "eyecolor", "blue" }
});
```

Or you can pass in an object that will be reflected -- property names will become dictionary keys.

```
profile.SetValues(new {
  mood = "Happy",
  eyecolor = "blue"
});
```

(Clearly, that last method presents issues with property naming. Your attribute keys would have to obey C# syntax rules -- no spaces, no hyphens, etc. If your keys violate those rules, then you'll need to use the `Dictionary` method.)

## Automating ID Retrieval

Normally, you need to pass in an ID to instatiate an object. This isn't ideal if you're instantiating the object in different locations in your code.

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

That will call the `IdProvider` method internally, and feed the result into the constructor, returning a populated profile. If your `IdProvider` returns `null`, then `GetCurrentUser` will return `null` as well.
