using UnityEngine;
using System.Collections;
// using Toki.Core;

public class SingleIndieObject<Type> : MonoBehaviour where Type : MonoBehaviour 
{
    /************************************************************************
     *	 	 	 	 	Private Variable Declaration	 	 	 	 	 	*
     ************************************************************************/
    private static bool _isQuitting = false;
	private static object _lock = new object();
	private static Type _inst;

    /************************************************************************
     *	 	 	 	 	Protected Variable Declaration	 	 	 	 	 	*
     ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Public Variable Declaration	 	 	 	 	 		*
     ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Getter & Setter Declaration	 	 	 	 	 		*
     ************************************************************************/
	public static Type To
	{
		get
		{
			if( _isQuitting )
			{
				return null;
			}

			lock( _lock )
			{
				if( _inst == null )
				{
					_inst = ( Type ) FindObjectOfType( typeof( Type ) );
					
					if( FindObjectsOfType( typeof( Type ) ).Length > 1 )
					{
						return _inst;
					}
					
					if( _inst == null )
					{
                        string typeName = typeof( Type ).ToString();
                        typeName = typeName.Substring( typeName.LastIndexOf('.') + 1 );
                        GameObject game = new GameObject( typeName );

						_inst = game.AddComponent<Type>();
                        DontDestroyOnLoad(game);
					}
					
					return _inst;
				}
			}

			return _inst;
		}
	}


    /************************************************************************
         *	 	 	 	 	Initialize & Destroy Declaration	 	 	 		*
         ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Life Cycle Method Declaration	 	 	 	 	 	*
     ************************************************************************/
    void OnDestory()
    {
        _isQuitting = true;
    }


    /************************************************************************
     *	 	 	 	 	Coroutine Declaration	 	  			 	 		*
     ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Private Method Declaration	 	 	 	 	 		*
     ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Protected Method Declaration	 	 	 	 	 	*
     ************************************************************************/


    /************************************************************************
     *	 	 	 	 	Public Method Declaration	 	 	 	 	 		*
     ************************************************************************/
}
