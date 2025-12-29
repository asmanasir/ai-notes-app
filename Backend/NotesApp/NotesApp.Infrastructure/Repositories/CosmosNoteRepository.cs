using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using NotesApp.Application.Interfaces;
using NotesApp.Domain.Entities;
using System.Net;

namespace NotesApp.Infrastructure.Repositories
{
    public class CosmosNoteRepository : ICosmosNoteRepository
    {
        private readonly Container _container;

        // Allowed sortable fields (Cosmos is case-sensitive!)
        private static readonly HashSet<string> AllowedOrderBy = new()
        {
            "createdAt",
            "updatedAt",
            "title"
        };

        public CosmosNoteRepository(
            CosmosClient cosmosClient,
            IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];

            if (string.IsNullOrWhiteSpace(databaseName) ||
                string.IsNullOrWhiteSpace(containerName))
            {
                throw new InvalidOperationException(
                    "CosmosDb configuration is missing DatabaseName or ContainerName.");
            }

            _container = cosmosClient
                .GetDatabase(databaseName)
                .GetContainer(containerName);
        }

        // =====================================================
        // CREATE
        // =====================================================
        public async Task AddAsync(Notes note)
        {
            if (string.IsNullOrWhiteSpace(note.UserId))
                throw new InvalidOperationException("UserId is required (Cosmos partition key).");

            if (string.IsNullOrWhiteSpace(note.Id))
                note.Id = Guid.NewGuid().ToString();

            await _container.CreateItemAsync(
                note,
                new PartitionKey(note.UserId)
            );
        }

        // =====================================================
        // READ ALL (by user)
        // =====================================================
        public async Task<IEnumerable<Notes>> GetAllAsync(string userId)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.userId = @userId"
            ).WithParameter("@userId", userId);

            var iterator = _container.GetItemQueryIterator<Notes>(query);
            var results = new List<Notes>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        // =====================================================
        // READ BY ID
        // =====================================================
        public async Task<Notes?> GetByIdAsync(string id, string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<Notes>(
                    id,
                    new PartitionKey(userId)
                );

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        // =====================================================
        // UPDATE
        // =====================================================
        public async Task UpdateAsync(Notes note)
        {
            if (string.IsNullOrWhiteSpace(note.UserId))
                throw new InvalidOperationException("UserId is required (Cosmos partition key).");

            await _container.UpsertItemAsync(
                note,
                new PartitionKey(note.UserId)
            );
        }

        // =====================================================
        // DELETE
        // =====================================================
        public async Task DeleteAsync(string id, string userId)
        {
            await _container.DeleteItemAsync<Notes>(
                id,
                new PartitionKey(userId)
            );
        }

        // =====================================================
        // PAGED QUERY
        // =====================================================
        public async Task<(IEnumerable<Notes> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string orderBy,
            string direction,
            string userId)
        {
            if (!AllowedOrderBy.Contains(orderBy))
                orderBy = "updatedAt";

            var order = direction?.ToLower() == "asc" ? "ASC" : "DESC";
            var offset = (page - 1) * pageSize;

            var queryText = $@"
                SELECT * FROM c
                WHERE c.userId = @userId
                ORDER BY c.{orderBy} {order}
                OFFSET @offset LIMIT @limit
            ";

            var query = new QueryDefinition(queryText)
                .WithParameter("@userId", userId)
                .WithParameter("@offset", offset)
                .WithParameter("@limit", pageSize);

            var iterator = _container.GetItemQueryIterator<Notes>(query);
            var items = new List<Notes>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                items.AddRange(response);
            }

            // Count query
            var countQuery = new QueryDefinition(
                "SELECT VALUE COUNT(1) FROM c WHERE c.userId = @userId"
            ).WithParameter("@userId", userId);

            var countIterator = _container.GetItemQueryIterator<int>(countQuery);
            var totalCount = 0;

            if (countIterator.HasMoreResults)
            {
                var response = await countIterator.ReadNextAsync();
                totalCount = response.FirstOrDefault();
            }

            return (items, totalCount);
        }
    }
}
