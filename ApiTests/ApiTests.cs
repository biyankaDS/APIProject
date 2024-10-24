using System.Net;
using System.Net.Http.Json;

namespace APITests{

    // define response classes
    public class ApiResponse
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public ApiData? data { get; set; }
    }

    public class ApiData
    {
        public string? brand { get; set; }
        public double? price { get; set; }
        public string? CPU_model { get; set; }
        public string? Hard_disk_size { get; set; }
    }

public class ApiTests
{
    private readonly HttpClient _client;
    private string? objectId; // store the objectId here for reuse

    public ApiTests()
    {
        _client = new HttpClient { BaseAddress = new Uri("https://api.restful-api.dev/") };
    }

    // Test 1: Get list of all objects
    [Fact]
    public async Task GetAllObjects()
        {

            var response = await _client.GetAsync("/objects");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var objects = await response.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(objects);
        }

    
    
    // Test 2: Add an object using POST  
    [Fact]
    public async Task AddObject()
        {
            //define objects
            var newObject = new
            {
                name = "Apple MacBook Pro 16",
                data = new
                {
                    brand = "Apple",
                    price = 1849.99,
                    CPU_model = "M1 Chip",
                    Hard_disk_size = "1 TB"
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/objects", newObject);

            // Assert Verify the POST request was successful
            Assert.True(postResponse.StatusCode == HttpStatusCode.Created || postResponse.StatusCode == HttpStatusCode.OK);

            var createdObject = await postResponse.Content.ReadFromJsonAsync<ApiResponse>();

            Assert.NotNull(createdObject);
            Assert.Equal("Apple MacBook Pro 16", createdObject?.name);

            objectId = createdObject?.id;  // Store the object ID for later use
            Assert.NotNull(objectId);
        }    


    // Test 3: Get a single object by ID
    [Fact]
        public async Task GetSingleObject()
        {
            // Ensure the object has been created
            if (objectId == null)
            {
                await AddObject();
            }

            var response = await _client.GetAsync($"/objects/{objectId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var obj = await response.Content.ReadFromJsonAsync<ApiResponse>();
            Assert.NotNull(obj);
            Assert.Equal("Apple MacBook Pro 16", obj?.name);
        }

        

    // Test 4: Update an object using PUT
    [Fact]
        public async Task UpdateObject()
        {
            // Ensure the object has been created
            if (objectId == null)
            {
                await AddObject();
            }

            var updatedObject = new
            {
                name = "iPad Air Pro2",
                data = new
                {
                    brand = "Apple",
                    price = 1399.99,
                    CPU_model = "A15 Chip",
                    Hard_disk_size = "1 TB"
                }
            };

            var response = await _client.PutAsJsonAsync($"/objects/{objectId}", updatedObject);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedResponseObject = await response.Content.ReadFromJsonAsync<ApiResponse>();
            Assert.NotNull(updatedResponseObject);
            Assert.Equal("iPad Air Pro2", updatedResponseObject?.name);
        }



    // Test 5: Delete an object using DELETE
    [Fact]
        public async Task DeleteObject
        ()
        {
            // Ensure the object has been created
            if (objectId == null)
            {
                await AddObject();
            }

            var response = await _client.DeleteAsync($"/objects/{objectId}");

            // Confirm deletion
            var getResponse = await _client.GetAsync($"/objects/{objectId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }



    }
}

