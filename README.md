# Trek Product Monitor

Development Exercise

# Observations and Comments

It's been a while since I spun up a new Winforms application from scratch. So, I tried to focus on interactions with Azure, threading and minimizing 
queries for product information over really diving into the user interface patterns, exception handling or logging and diagnostics. 
If that type of work is significant then let me know and I can flush those parts out more.

I did take the time to wire up Structure Map for dependency injection since we did discuss that last week. 


# Server Side Updates
Not knowing what else the server side processes and data model are used for it's hard to say that anything should change but 
simply looking from the perspective of this application's use cases the following items would make this application easier or more efficient.

1. Add User Friendly Message to the queue update message - This would remove the requirement to potentially query for the product information on every update
and any attempt to cache or reduce those queries.  The down side is that you end up with the server creating a key piece of the user interface.

2. Create a new table to store the most recent product updates - This would allow us to send a single query to Azure to retrieve the last 
50 products that were updated without relying on the queue messages. The Row Key of this table would have to incorporate a timestamp to allow
for time based ranage queries. 
This approach would allow multiple clients to display a range of updates at the same time instead of a message being removed from the queue
when the client reads it.
This could also add some of the Vendor Product data to the table potentially reducing the number of calls required to display update messages.
Similar to the denormalization described in step 1

3. VendorProduct table Partition Key - If designing the server system I would want to have a discussion about removing the "Product_" prefix 
from the Row Key and add it as a suffix to the Partition Key. While this would generate pretty much the same primary key in Azure under the hood 
it would allow us to easily identify all the products for a single vendor without doing a like comparision on the  Row Key. There could be reason
to value partitioning all of a vendor's data together not simply the vendor product data. 


