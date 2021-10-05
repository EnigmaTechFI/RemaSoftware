-- operations
SET IDENTITY_INSERT [dbo].[Operations] ON
INSERT INTO Operations 
  ([OperationID], [Name], [Description]) VALUES
  (1, N'Pulimentatura', N'Pulimentatura'),
  (2, N'Lucidatura', N'Lucidatura'),
  (3, N'Foratura', N'Foratura')
  (4, N'Smerigliatura', N'Smerigliatura'),
  (5, N'Montaggio', N'Montaggio'),
  (6, N'Lavaggio', N'Lavaggio')
SET IDENTITY_INSERT [dbo].[Operations] OFF

-- clients
SET IDENTITY_INSERT [dbo].[Clients] ON
INSERT INTO [dbo].[Clients]
           (ClientID, Name, P_Iva, Street, StreetNumber, Cap, City, State)
     VALUES
           (1, 'Cliente1', '123412341', 'Via Antani', '123', '50100', 'Firenze', 'FI'),
           (2, 'Cliente2', '456456456', 'Via Antanetto', '456', '20100', 'Milano', 'MI'),
           (3, 'Cliente3', '987987978', 'Via Antanuccio', '555', '00100', 'Roma', 'RM')
SET IDENTITY_INSERT [dbo].[Clients] OFF

-- orders
SET IDENTITY_INSERT [dbo].[Orders] ON
INSERT INTO Orders
           (OrderID, Name, ClientID, Number_Piece, DataIn, DataOut, SKU, Image_URL, Pdf_URL, Description, Flag_Fattureincloud, Price_Tot, Price_Uni)
     VALUES
           (1, 'Ordine1', 1, 100, getdate(), getdate(), '001', '','','Ordine 100 pezzi', 0, 1000, 10),
           (2, 'Ordine2', 2, 150, getdate(), getdate(), '002', '','','Ordine 150 pezzi', 0, 15000, 100),
           (3, 'Ordine3', 3, 200, getdate(), getdate(), '003', '','','Ordine 200 pezzi', 0, 4000, 20)
SET IDENTITY_INSERT [dbo].[Orders] OFF

-- magazzino
SET IDENTITY_INSERT [dbo].[Warehouse_Stocks] ON
INSERT INTO [dbo].[Warehouse_Stocks]
           (Warehouse_StockID, Name, Brand, Number_Piece, Price_Tot, Price_Uni, Size)
     VALUES
           (1, 'Forbici', 'Marca n1', 1, 100, 100, 'U'),
           (2, 'Smerigliatrice', 'Marca n2', 1, 1000, 1000, 'U'),
           (3, 'Scotch', 'Marca n3', 10, 3, 30, 'U')
SET IDENTITY_INSERT [dbo].[Warehouse_Stocks] OFF