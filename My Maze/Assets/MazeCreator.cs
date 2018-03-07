using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 21x21칸에서 벽공간 길공간 벽공간 길공간 벽공간의 형태로 되어있다.
   1.[0][0]의 블록부터 시작한다. 뱡향을 선택하고 그 방향으로 +2칸의 위치가 0인지, 1인지를 확인한다.
   2. 1일 경우 +1칸에 벽을 세운다. 그리고 다시 돌아와 다른 방향을 선택한다. 모든 방향을 처리하면 코드는 종료된다.[x(i)번째 줄의][y(j)번째 칸]
*/

public class MazeCreator : MonoBehaviour {
	// 벽 블록과 길 블록 변수를 생성한다
	public GameObject wallBlock,pathBlock;
	// 블록의 색을 지정한다
	public Material color;
	private GameObject[,] clone = new GameObject[size,size];
	public const int size = 21;
	const int unDefined = 0;
	const int wall = 1;
	const int path = 2;
	const int EAST = 3;
	const int WEST = 4;
	const int SOUTH = 5;
	const int NORTH = 6;

	int[,] greed=new int[size,size];
	void setWall(){
		for(int i=0; i<size; i++){
			greed[0,i] = wall; //왼쪽라인
			greed[size-1,i] = wall; //오른쪽라인
			greed[i,0] = wall; //상단라인
			greed[i,size-1] = wall; //하단라인
		}
		for(int row=0; row<size; row+=2){
			for(int col=0; col<size; col+=2){
				greed[row,col] = wall; //모서리 초기화
			}
		}
		for(int row=0;row<size;row++){
        	for(int col=0;col<size;col++){
            	if(greed[col,row]==2){
            	    greed[col,row]=0;
           		}
			}
        }
    }
	
	void makePath(int myX, int myY, int pathDirection){
		int[] directions = new int[3];
		for(int k=0; k<3; k++){
			while(true){  
				int n = Random.Range(3,7);
				for(int j=0; j<3; j++){
					if(n==directions[j] || n==pathDirection){
						n = 0;
						break;
					}
				}
				if(n!=0){
					directions[k] = n;
					break;
				}
			}
		}
		int checkX, checkY;
		int wallX, wallY;
		int nextDir=0;

		for(int i=0; i<3; i++){
			checkX = myX;
			checkY = myY;
			wallX = myX;
			wallY = myY;

			switch(directions[i]){
				case EAST:
					checkX = myX + 2;
					wallX = myX + 1;
					nextDir = WEST;
					break;
				case WEST:
					checkX = myX - 2;
					wallX = myX - 1;
					nextDir = EAST;
					break;
				case SOUTH:
					checkY = myY - 2;
					wallY = myY - 1;
					nextDir = NORTH;
					break;
				case NORTH:
					checkY = myY + 2;
					wallY = myY + 1;
					nextDir = SOUTH;
					break;
			}

			if(0 < checkX && checkX < size && 0 < checkY && checkY < size){
				if(greed[checkX,checkY]==unDefined){
					greed[checkX,checkY] = path;
					makePath(checkX,checkY,nextDir);
				}else{
					greed[wallX,wallY] = wall;
				}
			}
		}
	}
	private void CreateWall (int x, int z) {
		clone[x,z]=(GameObject)Instantiate(wallBlock,new Vector3(x*2,0,z*2),Quaternion.EulerRotation(0,0,0));
		if(x%2==1||z%2==1){
			clone[x,z].GetComponent<Renderer>().material = color;
		}
	}	
	private void CreatePath (int x, int z) {
		clone[x,z]=(GameObject)Instantiate(pathBlock,new Vector3(x*2,0,z*2),Quaternion.EulerRotation(0,0,0));
		if(x%2==1||z%2==1){
			clone[x,z].GetComponent<Renderer>().material = color;
		}
	}
	// Use this for initialization

	void Start() {
		greed[1,1] = 2;
		makePath(1,1,5);
		setWall();
		for(int row=0;row<size;row++){
        	for(int col=0;col<size;col++){
            	if(greed[col,row]==1){   
					CreateWall(col,row);
				}else{
					CreatePath(col,row);
				}
			}
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
