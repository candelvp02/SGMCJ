using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public sealed class AppoinmentRepository : BaseRepository<Appointment>, IAppoinmentRepository
    {
        private readonly SGMCJDbContext context;

        public AppoinmentRepository(SGMCJDbContext context): base(context)
        {
            
            this.context = context;
        }
        public override Task<Appointment> AddAsync(Appointment entity)
        {
            // You can add custom logic here before adding the entity
            return base.AddAsync(entity);
        }
    }
}
