using AutoMapper;
using rpg.Dtos.Character;
using rpg.Dtos.Fight;
using rpg.Dtos.Skill;
using rpg.Dtos.User;
using rpg.Dtos.Weapon;

namespace rpg
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //convertir de este -> este
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
            CreateMap<Character, HighscoreDto>();
            CreateMap<User, GetUserDto>();
        }
    }
}
