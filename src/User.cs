public class User
{
	public static string KeyField;
	public static string ApiKey;

	public Dictionary<string, object> Attributes { get; init; } = new();
	public string Id { get; set; } 
	
	public User(string id)
	{
		EnsureInit();
		
		Id = id;

		var result = GetClient().GetStringAsync($"https://api.zaius.com/v3/profiles?{KeyField}={id}").Result;
		foreach(var pair in JsonSerializer.Deserialize<JsonElement>(result).GetProperty("attributes").EnumerateObject())
		{
			Attributes.Add(pair.Name, pair.Value.ToString());	
		}
	}
	
	public object GetValue(string key)
	{
		EnsureInit();

		return Attributes.GetValueOrDefault(key);		
	}
	
	public void SetValue(string key, object value)
	{
		EnsureInit();
		
		Attributes[key] = value;
		
		var valuesToSet = new Dictionary<string, object>();
		valuesToSet.Add(key, value);
		valuesToSet[KeyField] = Id;

		var response = GetClient().PostAsync($"https://api.zaius.com/v3/profiles", JsonContent.Create(new { attributes = valuesToSet })).Result;
		
		if(((int)response.StatusCode).ToString()[0] != '2')
		{
			throw new Exception($"Service returned an error: {response.Content.ReadAsStringAsync().Result}");
		}
	}
	
	private static HttpClient GetClient()
	{
		var h = new HttpClient();
		h.DefaultRequestHeaders.Add("x-api-key", "W4WzcEs-ABgXorzY7h1LCQ.aqx2ho-xztHtfG0A0McOMxA6_AUGUPF09H-aDGLOkzM");
		return h;
	}
	
	private static void EnsureInit()
	{
		if(string.IsNullOrWhiteSpace(KeyField))
		{
			throw new Exception("You must set a KeyField value");
		}

		if (string.IsNullOrWhiteSpace(ApiKey))
		{
			throw new Exception("You must set an ApiKey value");
		}
	}
}
