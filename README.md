This project demonstrates how to create an Index for MongoDb Documents.

Home.razor has 2 boxes.
One to create a collection (button name: Create Opov Event).
The other to insert a document into the collection (button name: Create Contest).

The method OpovDbAccessService.AddIndexOnNameField() is used to create the Index.

The OpovEvent is a collection.
The Contest Document is added to the OpovEvent collection with an Index.
