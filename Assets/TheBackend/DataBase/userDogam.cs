using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using BackEnd;

public class userDogam 
{
    public int userId=0;
    public int noonsongId=0;
    public int count=0;
    public string location=string.Empty;


    public override string ToString()
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"userId: {userId}");
        result.AppendLine($"noonsongId: {noonsongId}");
        result.AppendLine($"count: {count}");
        result.AppendLine($"location: {location}");

        return result.ToString();
    }
}

  