namespace Eduhunt.Models.Contracts
{
    public class _Base : IHasId, IHasSoftDelete
    {

        public _Base()
        {
            this.Id = Guid.NewGuid().ToString();
            this.IsNotDeleted = true;
        }

        //IHasId
        public string Id { get; set; }

        //IHasSoftDelete
        public bool IsNotDeleted { get; set; }
    }
}
