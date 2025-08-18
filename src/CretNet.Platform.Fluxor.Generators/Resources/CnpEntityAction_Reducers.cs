// Reducers
public class [[ShortClassName]]Reducers
{
    [ReducerMethod]
    public static [[StateClassName]] Action([[StateClassName]] state, [[ClassName]] action)
    {
        if (!action.SaveToState)
            return state;
        
        var newState = state with
        {
            [[ActionReducerSource]]
        };
        
        return newState;
    }

    
    [ReducerMethod]
    public static [[StateClassName]] Success([[StateClassName]] state, [[SuccessClassName]] action)
    {
        if (!action.SourceAction.SaveToState)
            return state;
        
        var newState = state with
        {
            [[SuccessReducerSource]]
        };
        [[CustomReducerSource]]
        return newState;
    }
    
    [ReducerMethod]
    public static [[StateClassName]] Failure([[StateClassName]] state, [[FailureClassName]] action)
    {
        if (!action.SourceAction.SaveToState)
            return state;
        
        var newState = state with
        {
            [[FailureReducerSource]]
        };
        
        return newState;
    }
}