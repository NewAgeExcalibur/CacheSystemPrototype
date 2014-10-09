Cache Prototype

Local cache and Distributed cache combination prototype.

To Make Caching Layer works faster ,smarter and more efficient, cache layer include tow main layer , Local(In Memory) and Distributed cache.

Only most frequent request object can go to local cache , otherwise distributed cache is gonna used.

Since local memory is so limited we need to create a clean up process for local cache, 
To be able to remove object that are not high demand in the last period of time.

Assumption: Data Never Changes.


