namespace RemaSoftware.WebApp.DTOs;

public class JsonResultBaseDTO
{
    public bool Result { get; set; }
    public string ToastMessage { get; set; }

    public JsonResultBaseDTO() { }
    
    public JsonResultBaseDTO(bool result, string toastMessage)
    {
        Result = result;
        ToastMessage = toastMessage;
    }
}