# CosmosDbChangeFeeds
CosmosDB changeFeeds implementation example

## ProviderAssignmentSync
Fires on CosmosDB change feed and make an Http Post request to an API to sync a provider.

## AssignmentHistorization
Fires on CosmosDB change feed and transfers the assignment to the assignment history collection inside another CosmosDB database. A new id is created for the historic item and the original assigment id is stored on "assigment_id" property in order to have the whole historical states for each assignment.
