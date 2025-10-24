﻿using Microsoft.EntityFrameworkCore;
using SGMCJ.Domain.Entities.Medical;
using SGMCJ.Domain.Repositories.Medical;
using SGMCJ.Persistence.Base;
using SGMCJ.Persistence.Context;

namespace SGMCJ.Persistence.Repositories.Medical
{
    public sealed class MedicalRecordRepository : BaseRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(HealtSyncContext context) : base(context) { }

        public override async Task<MedicalRecord?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public Task<MedicalRecord?> GetByIdAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id, out int recordId))
                return Task.FromResult<MedicalRecord?>(null);

            return GetByIdAsync(recordId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(int patientId)
            => await _dbSet.Where(m => m.PatientId == patientId).ToListAsync();

        public async Task<IEnumerable<MedicalRecord>> GetByDoctorIdAsync(int doctorId)
            => await _dbSet.Where(m => m.DoctorId == doctorId).ToListAsync();

        public async Task<MedicalRecord?> GetByIdWithDetailsAsync(int recordId)
        {
            return await _dbSet
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .FirstOrDefaultAsync(m => m.RecordId == recordId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdWithDetailsAsync(int patientId)
        {
            return await _dbSet
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Where(m => m.PatientId == patientId)
                .ToListAsync();
        }
        public override async Task UpdateAsync(MedicalRecord record)
        {
            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        async Task<MedicalRecord> IMedicalRecordRepository.UpdateAsync(MedicalRecord record)
        {
            _context.Entry(record).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return record;
        }
    }
}