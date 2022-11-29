--BEGIN TRANSACTION;
--INSERT INTO Products (ClientID, Image_URL, Name, Price_Uni, SKU)
--SELECT distinct o.ClientID, o.Image_URL, o.Name, o.Price_Uni, o.SKU
--FROM Orders o 
--COMMIT;

--BEGIN TRANSACTION;
--INSERT INTO Ddt_In(ProductID, DataIn, DataOut, Code, FC_Ddt_In_ID, Number_Piece, Status, Note, Description, Priority, IsReso)
--SELECT s.ProductID, o.DataIn, o.DataOut, o.DDT, o.ID_FattureInCloud, o.Number_Piece, o.Status, o.Note, o.Description, 1, 0
--FROM Orders o 
--	inner join Products s on o.SKU = s.SKU AND o.ClientID = s.ClientID
--COMMIT;

--TODO: popolare la product operation

--BEGIN TRANSACTION;

--with tmp as (
--SELECT s.SKU, S.ClientID, o.Ddt_In_ID
--	FROM Ddt_In o 
--	inner join Products s on o.ProductID = s.ProductID
--)

--INSERT INTO ProductOperations
--SELECT *
--FROM tmp t
--	inner join Orders s on t.SKU = s.SKU AND s.ClientID = t.ClientID
--COMMIT;