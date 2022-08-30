using Amazon.S3;
using Amazon.S3.Model;
using TasksService.Controllers;

namespace TasksService;

public class TasksS3Repository : ITasksRepository
{
    private readonly IAmazonS3 _client;
    private const string BucketName = "dror.tasks.new";

    public TasksS3Repository(IAmazonS3 client)
    {
        _client = client;
    }

    public async Task<IEnumerable<TodoWithId>> GetAllTasks()
    {
        var request = new ListObjectsRequest
        {
            BucketName = BucketName
        };
        var response = await _client.ListObjectsAsync(request);
        var resultList = new List<TodoWithId>();
        foreach (var s3Object in response.S3Objects)
        {
            var result = await GetTaskById(s3Object.Key);

            resultList.Add(
                new TodoWithId
                {
                    Id = s3Object.Key,
                    Title = result.Title,
                    Content =
                        result.Content
                });
        }

        return resultList;
    }

    public async Task<Todo> GetTaskById(string id)
    {
        var result = new Todo();

        var getObjectRequest = new GetObjectRequest
        {
            Key = id,
            BucketName = BucketName
        };
        using var getObjectResponse = await _client.GetObjectAsync(getObjectRequest);
        await using var responseStream = getObjectResponse.ResponseStream;
        using var reader = new StreamReader(responseStream);
        result.Title = getObjectResponse.Metadata["x-amz-meta-title"];

        result.Content = await reader.ReadToEndAsync();

        return result;
    }

    public async Task<string> CreateNewTask(Todo value)
    {
        var newId = Guid.NewGuid().ToString();

        PutObjectRequest request = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = newId,
            ContentBody = value.Content
        };

        request.Metadata.Add("x-amz-meta-title", value.Title);

        await _client.PutObjectAsync(request);

        return newId;
    }
}