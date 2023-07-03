using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;

namespace JiraSchedulingConnectAppService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly JiraDemoContext _context;

        public EquipmentController(JiraDemoContext context)
        {
            _context = context;
        }

        // GET: api/Equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()
        {
          if (_context.Equipments == null)
          {
              return NotFound();
          }
            return await _context.Equipments.ToListAsync();
        }

        // GET: api/Equipment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Equipment>> GetEquipment(int id)
        {
          if (_context.Equipments == null)
          {
              return NotFound();
          }
            var equipment = await _context.Equipments.FindAsync(id);

            if (equipment == null)
            {
                return NotFound();
            }

            return equipment;
        }

        // PUT: api/Equipment/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEquipment(int id, Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return BadRequest();
            }

            _context.Entry(equipment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Equipment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Equipment>> PostEquipment(Equipment equipment)
        {
          if (_context.Equipments == null)
          {
              return Problem("Entity set 'JiraDemoContext.Equipments'  is null.");
          }
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEquipment", new { id = equipment.Id }, equipment);
        }

        // DELETE: api/Equipment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            if (_context.Equipments == null)
            {
                return NotFound();
            }
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EquipmentExists(int id)
        {
            return (_context.Equipments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
