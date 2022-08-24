using rpg.Dtos.Character;
using rpg.Dtos.Weapon;

namespace rpg.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);

    }
}
