using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.Repositories.Interfaces;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<IEnumerable<HolidayDTO>> GetAllHolidaysAsync()
        {
            var holidays = await _holidayRepository.GetAllHolidaysAsync();
            return holidays.Select(MapToDTO);
        }

        public async Task<HolidayDTO> GetHolidayByIdAsync(int id)
        {
            var holiday = await _holidayRepository.GetHolidayByIdAsync(id);
            if (holiday == null) throw new KeyNotFoundException("Holiday not found");

            return MapToDTO(holiday);
        }

        public async Task<HolidayDTO> CreateHolidayAsync(HolidayDTO dto)
        {
            if (dto == null) throw new ArgumentException("Invalid request data");

            var holiday = new Holiday
            {
                Date = dto.Date,
                Name = dto.Name
            };

            var created = await _holidayRepository.CreateHolidayAsync(holiday);
            return MapToDTO(created);
        }

        public async Task UpdateHolidayAsync(int id, HolidayDTO dto)
        {
            if (dto == null || id != dto.Id) throw new ArgumentException("Invalid request data");

            var holiday = await _holidayRepository.GetHolidayByIdAsync(id);
            if (holiday == null) throw new KeyNotFoundException("Holiday not found");

            holiday.Date = dto.Date;
            holiday.Name = dto.Name;

            await _holidayRepository.UpdateHolidayAsync(holiday);
        }

        public async Task DeleteHolidayAsync(int id)
        {
            var holiday = await _holidayRepository.GetHolidayByIdAsync(id);
            if (holiday == null) throw new KeyNotFoundException("Holiday not found");

            await _holidayRepository.DeleteHolidayAsync(holiday);
        }

        private static HolidayDTO MapToDTO(Holiday h)
        {
            return new HolidayDTO
            {
                Id = h.Id,
                Date = h.Date,
                Name = h.Name
            };
        }
    }
}
