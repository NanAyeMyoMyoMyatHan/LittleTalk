using LittleTalk.Models.Common;
using LittleTalk.Shared;

namespace LittleTalk.Models.Features.Students;

public class SearchStudentRequestModel : PaginationRequestModel
{
    public int? LearningLevelId { get; set; }
    public int? LevelId 
    { 
        get => LearningLevelId; 
        set => LearningLevelId = value; 
    }
    public bool? IsActive { get; set; }
}

