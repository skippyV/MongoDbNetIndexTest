
Home.razor has 2 boxes.
One to create a collection (button name: Create Opov Event).
The other to insert a document into the collection (button name: Create Contest).

The method OpovDbAccessService.AddIndexOnNameField() is used to create the Index.

However, if I create 2 documents with the same name, they are both inserted.
I'd expect only one to exist in the collection.
Because the Index should have prevented another Document, with the same name, from getting inserted.