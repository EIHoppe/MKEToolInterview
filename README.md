# Overview

This repo contains my implementation of a take-home assignment for Milwaukee Tool. The task was to create a restaurant review API.

# Setup

The project is set up to run out of Visual Studio (developed/tested with VS 2019 Community), and is deployable to AWS via a CloudFormation template using the AWS VS Extension.

Some caveats about deployment: you must create a DynamoDB table w/ the following parameters:
- Table Name: mketool-restaurants
- Partition Key: RestaurantId
- Sort Key: SortKey

If you want the test project to run correctly, a parallel table must be made with table name mketool-restaurants-test as well.

Additionally, the IAM policy made by deploying must be given DynamoDB permissions.

This was a "cut for time" issue; ideally, the cloud formation would ideally set all of this up, but it was descoped in favor of spending time on creating some automated tests.

# Insomnia Collection

An Insomnia collection has been included in the repo to better enable exercising the API.

There are two environments, one which corresponds to running the project locally out of IIS Express / directly from VS ("Local"), and the other that corresponds to the deployment in AWS (as of committing) ("AWS"). Due to AWS resources being used, the latter is not guaranteed to be up except when being demonstrated.

To use the collection, import the JSON file into your local Insomnia client.

# General Caveats / "Cut for Time"s / TODOs
Several things were cut to avoid this spiraling into a multi-day odyssey. Those include, but are not limited to:
- As mentioned above, deployment could stand to be cleaned up significantly; this was very much a "get something out there" level of thought put into it.
- Similarly, in a real environment you'd want proper CI/CD and tie in the test running as part of the pipeline.
  - Bonus points: the test database could be spun up / deleted as part of that pipeline as well
- The Hours field on the Restaurant model is a simple text field; ideally this would be some sort of more complex (presumably DateTime(Offset)-based) data structure to better enable formatting and timezone information.
- The very small DynamoDB helper can only access single-valued attributes off of documents; if that were expanded adding list handling would be good too.
- There are some hardcoded values, particularly around the SortKeys for the documents.
- Interfaces should be set up for the services, so they'd be better abstractable in the future.
- All of the DynamoDB access should be pushed into a DAL such that it could get swapped out for different database tech later.
- Auth was completely ignored; the API is completely open.
- All testing was at the integration level; with some of the above changes, some unit tests would also be possible (and probably used for, e.g. the average rating calculations).
