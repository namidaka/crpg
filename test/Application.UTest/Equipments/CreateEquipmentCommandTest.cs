using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Trpg.Application.Equipments.Commands;
using Trpg.Domain.Entities;

namespace Trpg.Application.UTest.Equipments
{
    public class CreateEquipmentCommandTest : TestBase
    {
        [Test]
        public async Task Basic()
        {
            var handler = new CreateEquipmentCommand.Handler(_db, _mapper);
            var e = await handler.Handle(new CreateEquipmentCommand
            {
                Name = "my sword",
                Price = 100,
                Type = EquipmentType.Body,
            }, CancellationToken.None);

            Assert.NotNull(await _db.Equipments.FindAsync(e.Id));
        }

        [Test]
        public void InvalidEquipment()
        {
            var validator = new CreateEquipmentCommand.Validator(_db);
            var res = validator.Validate(new CreateEquipmentCommand
            {
                Name = "",
                Price = 0,
                Type = (EquipmentType) 500,
            });

            Assert.AreEqual(3, res.Errors.Count);
        }

        [Test]
        public async Task DuplicateName()
        {
            _db.Equipments.Add(new Equipment
            {
                Name = "toto",
            });
            await _db.SaveChangesAsync();

            var validator = new CreateEquipmentCommand.Validator(_db);
            var res = validator.Validate(new CreateEquipmentCommand
            {
                Name = "toto",
                Price = 100,
                Type = EquipmentType.Body,
            });

            Assert.AreEqual(1, res.Errors.Count);
        }
    }
}