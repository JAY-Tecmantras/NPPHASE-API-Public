namespace NPPHASE.Data.Interface
{
    public interface IAuditabeEntity
    {
        DateTimeOffset CreationDate { get; set; }
        string? CreatedBy { get; set; }
        DateTimeOffset? ModificationDate { get; set; }
        string? ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
    }
}
