using LittleTalk.Model.Common;

namespace LittleTalk.Model.Features.Students;

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
