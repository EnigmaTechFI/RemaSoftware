using RemaSoftware.Domain.Models;

namespace RemaSoftware.Domain.Services
{
    public interface IOrderService
    {
        int GetTotalProcessedPiecese();
        int GetCountOrdersNotExtinguished();
        decimal GetLastMonthEarnings();
        List<Ddt_In> GetOrdersNearToDeadlineTakeTop(int topSelector);
        List<Ddt_In> GetAllOrdersNearToDeadline();
        Batch GetBatchById(int batchId);
        Batch GetBatchByProductIdAndOperationList(int productId, List<int> operationId);
        Batch CreateBatch(Batch batch); 
        Ddt_In CreateDDtIn(Ddt_In ddt_In);
        Ddt_In UpdateDDtIn(Ddt_In ddt_In);
        List<Ddt_In> GetAllDdtIn();
        List<Ddt_In> GetDdtInActive();
        List<Ddt_In> GetDdtInWorkingByClientId(int Id);
        List<Ddt_In> GetDdtInEnded(int year);
        List<Ddt_In> GetDdtInStockByClientId(int Id);
        Ddt_In GetDdtInById(int id);
        List<Ddt_Out> GetDdtOutsByClientIdAndStatus(int id, string status);
        Ddt_Out CreateNewDdtOut(Ddt_Out ddtOut);
        List<Ddt_Out> GetDdtOutsByStatus(string status);
        Ddt_Out GetDdtOutById(int id);
        Task UpdateDdtOut(Ddt_Out ddt);
        Ddt_Out GetDdtOutsById(int id);
        Ddt_Out CreateDDTOut(Ddt_Out ddtOut);
        void UpdateDdtAssociationByIdWithNewDdtOut(int ddtAssociationId, int ddtOutDdtOutId);
        void DeleteDDT(Ddt_In ddt);
        void DeleteSubBatch(SubBatch subBatch);
        void DeleteBatch(Batch batch);
        List<Ddt_In> GetDdtInActiveByClientId(int clientId);
        List<Ddt_In> GetDdtInEndedByClientId(int clientId);
        void CreateDDTAssociation(Ddt_Association ass);
        List<Label> GetLastLabelOut();
        void CreateNewLabelOut(Label label);
        List<Batch> GetAllBatch();
        Ddt_Association GetDDTAssociationById(int id);
        void DeleteDDTAssociation(Ddt_Association ddtAssociation);
        void DeleteDDTOut(Ddt_Out ddtOut);
        Ddt_Supplier CreateNewDdtSupplier(Ddt_Supplier modelDdtSupplier);
        void CreateNewDdtSupplierAssociation(DDT_Supplier_Association entity);
        void CreateNewDdtSuppliersAssociation(List<DDT_Supplier_Association> ddtSupplierAssociations);
        void UpdateDDtSupplier(Ddt_Supplier ddtSupplier);
        Ddt_Supplier GetDdtSupplierById(int modelDdtSupplierId);
        void UpdateDDtInRange(List<Ddt_In> ddts);
        void DeleteDDTAssociations(List<Ddt_Association> ddtDdtAssociations);
        public List<Ddt_Association> GetDDTAssociationsByDDTIn(Ddt_In Id);
    }
}