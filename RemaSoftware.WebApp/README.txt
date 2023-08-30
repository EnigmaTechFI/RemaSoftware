per la pubblicazione ci sono 2 profili (è indifferente quale usare):
 1) FTPProfile: pubblica direttamente da VS sull'hosting, ma è molto lento
 2) FolderProfile (consigliato): pubblica in una cartella locale, quando ha finito bisogna caricare a mano (per es. con filezilla) i file sull'hosting

NB: prima di pubblicare assicurarsi di essere in Release e aver selezionato il launcProfile "IIS Express-Prod"

TODO: 
1. Modifica dei model + migration;
2. Creazione di un prodotto (Stampa del qr code collegato ad id prodotto);
3. Lista  (Tra le azioni nell'ultima colonna dovrà essere presente link per pagina per effettuare il ricarico del prodotto);
4. Modifica prodotto;
5. Pagina dettaglio prodotto con storico carico / scarico (Vedi cliente)
6. Scarico del prodotto tramite lettura QRCODE;
7. Email all'amministrazione in caso di superamento soglia minima;

MODELLO:
1. Eliminazione: Marca, taglia;
2. Aggiunta: Id Fornitore (con lista virtuale di prodotti nel modello del fornitore), Codice prodotto (deve essere quello dato dal fornitore e lo riutilizzeremo per il riordine automatico), Unità di misura (Creare una classe costant static e mostrarle in pagina come dropdown), soglia di riordino;
3. Creazione modello storico prodotto: Dentro il prodotto ci dovrà essere una lista virtuale relativa allo storico;


Per Migration - DA TERMINAL:
1. cd .\RemaSoftware.Domain\
2. dotnet ef migrations add <nome-migration> --startup-project ../RemaSoftware.WebApp/RemaSoftware.WebApp.csproj
3. Controllare nella cartella del progetto cosa ha generato la migration
4. dotnet ef database update --startup-project ../RemaSoftware.WebApp/RemaSoftware.WebApp.csproj
5. Le migration non vengono aggiunte su git, vanno spostate a mano

NOTE:
- La gestione del prezzo rifarsi alla pagina new order in quanto il prezzo in vista è una stringa che poi viene convertita nell'helper. Bloccare il punto ed usare solo la virgola come separatore decimale. 
- La creazione di services e di helper deve essere registrata nello startup come gli altri altrimenti da eccezione (Dependency Injection)