using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Character;
using rpg.Models;
using rpg.Services.BaseClass;
using System.Security.Claims;

namespace rpg.Services.CharacterService
{
    public class CharacterService : GenericRepository<Character>, ICharacterService 
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        //constructor por automapper
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _mapper = mapper;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User //permite obtener el id del token que viene en los headers
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();
            //var dbCharacters = await _context.Characters
            //     .Include(c => c.Weapon) //TO DISPLAY WEAPONS IN RESPONSE
            //     .Include(c => c.Skills) //TO DISPLAY SKILLS IN RESPONSE //TO ADD SUBCHILDREN WE WOULD USE .ThenInclude( s => s.SideEffects)
            //     .Where(c => c.User.Id == GetUserId())
            //     .ToListAsync();
            //response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            //return response;
            try
            {
                var dbCharacters =  await GetAll()
                                    .Include(c => c.Weapon)
                                    .Include(c => c.Skills)
                                    .Where(c => c.User.Id == GetUserId())
                                    .ToListAsync();
                response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                return response;
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {

            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            //var dbCharacter = await _context.Characters
            //     .Include(c => c.Weapon) //TO DISPLAY WEAPONS IN RESPONSE
            //     .Include(c => c.Skills) //TO DISPLAY SKILLS IN RESPONSE //TO ADD SUBCHILDREN WE WOULD USE .ThenInclude( s => s.SideEffects)
            //     .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            //serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            //if (serviceResponse.Data == null)
            //{
            //    serviceResponse.Message = "The id does not correspond to any character associated with user logged";
            //    serviceResponse.Success = false;
            //}
            //return serviceResponse;
            try
            {
                var dbCharacter = await GetByCondition(c => c.Id == id && c.User.Id == GetUserId())
                                    .Include(c => c.Weapon)
                                    .Include(c => c.Skills)
                                    .FirstOrDefaultAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
                if (serviceResponse.Data == null)
                {
                    serviceResponse.Message = "The id does not correspond to any character associated with user logged";
                    serviceResponse.Success = false;
                }
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            //character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId()); //we make sure tha char is created to a corresponding User
            //_context.Characters.Add(character);
            //await _context.SaveChangesAsync();
            //serviceResponse.Data = await _context.Characters
            //    .Where(c => c.User.Id == GetUserId())
            //    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            //return serviceResponse;
            try
            {
                character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId()); //we make sure tha char is created to a corresponding User
                await CreateAsync(character);
                serviceResponse.Data = await GetAll()
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return serviceResponse;
            }
        }

        

       

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            //try
            //{
            //    var character = await _context.Characters
            //        .Include(c => c.User)  //we need this in order to include the info of nested proerties like Character that has user associated
            //        .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id && c.User.Id == GetUserId());

                
            //    if (character == null)
            //    {
            //        serviceResponse.Data = null;
            //        serviceResponse.Success = false;
            //        serviceResponse.Message = "Character with such id could not be found!";
            //        return serviceResponse;
            //    }
            //    //character.Name = updatedCharacter.Name;
            //    //character.HitPoints = updatedCharacter.HitPoints;
            //    //character.Strength = updatedCharacter.Strength;
            //    //character.Defense = updatedCharacter.Defense;
            //    //character.Intelligence = updatedCharacter.Intelligence;
            //    //character.Class = updatedCharacter.Class;
            //    _mapper.Map(updatedCharacter, character); //CON ESTA LINEA HACEMOS LO MISMO DE ARRIBA de propiedad por propiedad
            //    await _context.SaveChangesAsync();

            //    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            //    serviceResponse.Message = "Succesfully updated character " + character!.Name;

            //} catch (Exception ex)
            //{
            //    serviceResponse.Success = false;
            //    serviceResponse.Message = "Error at updating a character";
            //}
            
            //return serviceResponse;
            try
            {
                var character = await GetByCondition(c => c.Id == updatedCharacter.Id && c.User.Id == GetUserId())
                                .Include(c => c.User)
                                .FirstOrDefaultAsync();
                if (character == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character with such id could not be found!";
                    return serviceResponse;
                }
                _mapper.Map(updatedCharacter, character);
                await UpdateAsync(character);
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                serviceResponse.Message = "Succesfully updated character " + character!.Name;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Error at updating a character";
            }
            return serviceResponse;

        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            //try
            //{
            //    Character character =  await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
            //    if(character == null)
            //    {
            //        serviceResponse.Data = null;
            //        serviceResponse.Message = "No character found with provided ID";
            //        serviceResponse.Success = false;
            //        return serviceResponse;
            //    }
            //    _context.Characters.Remove(character);
            //    await _context.SaveChangesAsync(); 


            //    serviceResponse.Message = "Succesfully deleted character " + character.Name;
            //    serviceResponse.Data = _context.Characters
            //        .Where(c => c.User.Id == GetUserId()) 
            //        .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();

            //}
            //catch (Exception ex)
            //{
            //    Console.Write(ex.ToString());
            //    serviceResponse.Success = false;
            //    serviceResponse.Message = "Error at deleting a character";
            //}

            //return serviceResponse;
            try
            {
                var character = await GetByCondition(c => c.Id == id && c.User!.Id == GetUserId())
                               .Include(c => c.User)
                               .FirstOrDefaultAsync();
                if (character == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "No character found with provided ID";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }
               await DeleteAsync(character);
                serviceResponse.Message = "Succesfully deleted character " + character.Name;
                serviceResponse.Data = await GetAll()
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                serviceResponse.Success = false;
                serviceResponse.Message = "Error at deleting a character";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await _context.Characters
                    .Include(c => c.Weapon) //TO DISPLAY WEAPONS IN RESPONSE
                    .Include(c => c.Skills) //TO DISPLAY SKILLS IN RESPONSE //TO ADD SUBCHILDREN WE WOULD USE .ThenInclude( s => s.SideEffects)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                    c.User.Id == GetUserId());

                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills.Add(skill);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetCharacterDto>(character);

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
