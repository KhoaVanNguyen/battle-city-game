using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BattleCity
{
    [Serializable]
    public class TankAI : Tank
    {
        /* TankAI - Note 1 */
        /*
         * Mảng 2 chiều heuristic này có kiểu số nguyên, kích thước thước 25 x 25 phần tử.
         
         * Tại vì bản đồ cơ sở có cấu trúc dữ liệu là một mảng 2 chiều có kiểu số nguyên và có kích thước
           26 x 26 phần tử mà ở đây ta chỉ sử dụng mảng có kích thước 25 x 25 phần tử.
         
         * Bản đồ cơ sở của trò chơi gồm 26 x 26 ô vuông nhỏ có kích thước 16 x 16 pixel.
           Mà vùng diện tích của xe tăng chiếm đến 4 ô vuông nhỏ có kíc thước 16 x 16 pixel.
           Hơn nữa ta lấy gốc tọa độ của xe tăng là ô vuông nhỏ bên trái trên cùng trong 4 ô vuông nhỏ đó
           -> Vùng tọa độ nguyên của xe tăng là (blockx, blocky) với blockx, blocky nằm trong khoảng từ 0 đến 24
         
         * Mảng 2 chiều heuristic này dùng để lưu trữ giá trị heuristic cho các ô vuông nhỏ trên bản đồ mà xe tăng có thể nằm trên đó.
           Ta quy ước như sau:
           + Cách tính giá trị như sau:
             Giả sử xe tăng đang nằm trên 4 ô vuông nhỏ có tọa độ nguyên góc trên trái là (blockx, blocky)
             Thì ta phải xét các ô có tọa độ (blockx, blocky), (blockx + 1, blocky), (blockx, blocky + 1), (blockx + 1, blocky + 1)
             - Nếu như tất cả 4 ô trên đều là những ô không có chướng ngại vật hoặc là ô cây cỏ thì chúng ta cho giá trị của ô
               có tọa độ là (blockx, blocky) là 1. Tại sao chúng ta lại cho là 1, để tiện cho việc tính quãng
               đường mà xe tăng đã đi. Thì khoảng các giữa mỗi bước đi 1 trong 4 hướng đều là 1 (đơn vị ô vuông nhỏ 16 x 16 pixel)
         
             - Nếu như có ít nhất 1 ô trong 4 ô trên là ô có thuộc tính là thép, chúng ta không thể xuyên phá nên trường hợp này
               coi như không thể nào tồn tại, bởi vì nó không thể xuyên phá thì làm sao mà có thể đứng trên đó được. Trong trường hợp này
               ta quy ước giá trị là 0. Quy ước 0 ở đây để đánh dấu ô đó khi gặp thì không có cạnh kề, tức là không thể mở rộng vào 
               ô đó được.
             
             - Nếu như có ít nhất 1 ô trong 4 ô trên là ô có thuộc tính là gạch, chúng ta có thể thể xuyên phá nên trường hợp này
               tồn tại, bởi vì nó có thể xuyên phá thì nó có thể đứng trên ô đó được đó được. Trong trường hợp này
               ta quy ước giá trị là tùy vào mục đích mà ta mong muốn xe tăng nó tìm đường đi như thế nào. Chúng tôi sẽ chú thích ở dưới,
               vui lòng bạn theo dõi
          * Nếu giá trị heuristic > 0 thì giá trị nó càng cao thì ta quy ước độ ưu tiên của nó càng thấp.
        */

        public int[,] impediment;
        public List<Point> pathss;

        /* TankAI - Note 2 */
        /*
         * Đây là hàm khởi tạo của lớp TankAI, sau khi thực hiện xong hàm này, nó sẽ gọi cơ sở của nó để bổ sung các giá trị thuộc
           tính của nó.
         
         * Ta quy ước ở đây có 3 loại xe tăng:
           Loại ở đây nó sẽ khởi tạo typeTank nó sẽ biết nó dùng frame ảnh nào trong sprites ảnh của trò chơi, tùy vào loại mà nó dùng tọa độ nào
           để vẽ hình biểu thị xe tăng.
           + typeTank 2: Ở đây ta quy ước nó có 1 mạng sống.
         
           + typeTank 3: Ở đây ta quy ước nó có 1 mạng sống.
         
           + typeTank 6: Ở đây ta quy ước nó có 3 mạng sống.
             - Nếu typeTank 6 nó bị bắn một phát thì nó sẽ đặt giá trị typeTank thành 5, và giảm mạng sống đi 1 đơn vị.
             - Nếu typeTank 5 nó bị bắn một phát thì nó sẽ đặt giá trị typeTank thành 4, và giảm mạng sống đi 1 đơn vị.
             - Nếu typeTank 4 nó sẽ đi!
         
         * Mỗi loại khác nhau ta quy ước tốc độ (đơn vị pixel theo thời gian) khác nhau. (Yếu tố này không quan trọng).
           Nó sẽ tạo độ phong phú các kiểu xe tăng trong trò chơi.
        */

        public TankAI(int type)
        {
            switch (type)
            { 
                case 2:
                    this.x = 0 * 16;
                    this.y = 0 * 16;
                    this.life = 1;
                    this.typeTank = 2;
                    this.deltaTime = 1.4f;
                    this.timeShotCricle = 40;
                    break;
                case 3:
                    this.x = 12 * 16;
                    this.y = 0 * 16;
                    this.life = 1;
                    this.typeTank = 3;
                    this.deltaTime = 1.4f;
                    this.timeShotCricle = 90;
                    break;
                case 4:
                    this.x = 24 * 16;
                    this.y = 0 * 16;
                    this.deltaTime = 0.7f;
                    this.life = 2;
                    this.typeTank = 6;
                    this.timeShotCricle = 80;
                    break;
            }

            this.currentDir = Global.up;
            this.isShield = true;
            this.impediment = new int [25, 25];
            this.pathss = new List<Point>();
        }

        /* TankAI - Note 3 */
        /*
         * Hàm checkAreaLeft(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công bên "trái" nhà mình
           hay không.
        */

        public bool checkAreaLeft(int qx, int qy)
        {
            if (qx >= MainForm.xAttackLeft && qx <= 9 && qy == 24)
            {
                return true;
            }
            return false;
        }

        /* TankAI - Note 4 */
        /*
         * Hàm checkAreaRight(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công bên "phải" nhà mình
           hay không.
        */

        public bool checkAreaRight(int qx, int qy)
        {
            if (qx <= MainForm.xAttackRight && qx >= 15 && qy == 24)
            {
                return true;
            }
            return false;
        }

        /* TankAI - Note 5 */
        /*
         * Hàm checkAreaDown(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công "trực diện"  nhà mình
           hay không.
        */

        public bool checkAreaDown(int qx, int qy)
        {
            if (qy <= 21 && qy >= MainForm.yAttackDown && qx == 12)
            {
                return true;
            }
            return false;
        }

        /* TankAI - Note 6 */
        /*
         * Hàm này tính giá trị heuristic tĩnh của một ô trên bản đồ cho xe tăng AI. Cách tính giá trị của nó đã
           nêu ở mục TankAI - Note 1
        */

        public int check4block16(int blockx, int blocky)
        {
            int result = 1, typeBlock;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    typeBlock = MainForm.map[blocky + j, blockx + i];
                    if (typeBlock == Global.steel)
                    {
                        return 0;
                    }
                    if (typeBlock == Global.wall || typeBlock == Global.wall1 || typeBlock == Global.wall2 || typeBlock == Global.wall3 || typeBlock == Global.wall4)
                    {
                        result = 669;
                    }
                }
            }
            return result;
        }

        /* TankAI - Note 7 */
        /*
         * Hàm preprocessorThink() nó là một hàm tiền xử lý trước khi chạy thuật toán tìm đường đi cho xe tăng của máy.
           nó thực hiện những việc sau:
           + Đầu tiên, nó tìm tọa độ nguyên (blockx, blocky) của xe tăng, tọa độ nguyên nó như thế nào thì chúng tôi đã giải thích
             ở trên mục TankAI - Note 1.
           + Thứ hai, nó sẽ lấy mảng 2 chiều 25 x 25 chứa các giá trị heuristic.
           + Thứ ba, nó sẽ truy tìm tất cả tọa độ xe tăng hiện hành trừ nó ra, để chi, nó sẽ gắn các tọa độ đó là những ô không thể vào
             được, vì xe tăng người ta đang đứng sao mà vào đó được.
        */

        public void preprocessorThink()
        {
            /* Tìm tọa độ nguyên (blockx, blocky) */
            blockx = roundBlock(x);
            blocky = roundBlock(y);
            /* Tìm tọa độ nguyên (blockx, blocky) */

            /* Tìm mảng heuristic */
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    impediment[i, j] = check4block16(i, j);
                }
            }

            /* Đánh dấu các chướng ngại vật là xe tăng hiện hành, trừ nó ra */
            foreach (var tank in MainForm.tanks.ToList())
            {
                if (tank != this)
                {
                    impediment[roundBlock(tank.x), roundBlock(tank.y)] = 0;
                    if (roundBlock(tank.x) + 1 < 25)
                    {
                        impediment[roundBlock(tank.x) + 1, roundBlock(tank.y)] = 0;
                    }
                    if (roundBlock(tank.y) + 1 < 25)
                    {
                        impediment[roundBlock(tank.x), roundBlock(tank.y) + 1] = 0;
                    }
                    if (roundBlock(tank.x) + 1 < 25 && roundBlock(tank.y) + 1 < 25)
                    {
                        impediment[roundBlock(tank.x) + 1, roundBlock(tank.y) + 1] = 0;
                    }
                }
            }
        }

        /* TankAI - Note 8 */
        /*
         * Hàm bool lookOver() nó sẽ xử lý những việc sau:
           + Thứ nhất nó xem xét xung quanh nó có có viên đạn nào có thể gây nguy hiểm cho nó không
             - Ở đây sao chúng ta có thể biết được những viên đạn nào xung quanh nó có thể gây nguy hiểm cho nó
               thì chúng tôi đã chuẩn bị sẵn mảng 2 chiều 25 x 25 phần tử. Chúng tôi đã tính toán và quy ước rằng
               # Nếu hướng viên đạn là UP thì hướng chúng ta cần bắn là DOWN, tương tự các trường hợp còn lại.
                 Bởi vậy giá trị của MainForm.bulletsDanger[blockx, blocky] nếu nó không được gắn hướng UP, DOWN, RIGHT, LEFT thì ta
                 hiểu rằng nó không có viên đạn nào gây nguy hiểm cho ô đó.
           + Thứ hai nó xem xét xung quanh nó có chiếc xe của phe người nằm trong tầm ngắm của nó không. Làm sao để biết có hay không?
             Thì chúng ta kiểm tra MainForm.peopleAttack[blockx, blocky] là 0 hay 1 tương ứng với người chơi 1 hoặc người chơi 2.
             Nó sẽ căn cứ tọa độ của nó và tọa độ của người chơi để đưa ra hướng và quyết định bắn.
             Cách tìm hướng như thế nào thì tôi có chú thích trong hàm dưới đây luôn!
        */

        public bool lookOver()
        {
            /*
             * Nếu dirDefender không là giá trị 0, 1, 2, 3 tương ứng với UP, DOWN, LEFT, RIGHT
               -> Tọa độ mà nó đang đứng không có bất kỳ viên đạn nào có thể gây nguy hiểm ở thời điểm hiện hành.
             
             * Nếu dirDefender là một trong bốn giá trị 0, 1, 2, 3 tương ứng với UP, DOWN, LEFT, RIGHT
               -> Nó đang bị nguy hiểm, và nó sẽ đưa ra quyết định bắn với hướng dirDefender.
             
             * MainForm.bulletsDanger được chúng tôi xử lý trong MainForm.
            */
            int dirDefender = MainForm.bulletsDanger[blockx, blocky];
            switch (dirDefender)
            {
                case Global.up:
                    this.currentDir = Global.up;
                    this.shot();
                    return true;
                case Global.down:
                    this.currentDir = Global.down;
                    this.shot();
                    return true;
                case Global.left:
                    this.currentDir = Global.left;
                    this.shot();
                    return true;
                case Global.right:
                    this.currentDir = Global.right;
                    this.shot();
                    return true;
            }

            /*
             * Ở đây nó xét tất cả người chơi đang tồn tại và nếu nó đang nằm trong vùng nguy hiểm của người chơi tạo ra thì
               nó sẽ xác định hướng và quyết định bắn để tự thủ.
             
             * MainForm.numberPeopleLive: Số người chơi còn sống trong trò chơi.
            */
            for (int i = 0; i < MainForm.numberPeopleLive; i++)
            {
                /*
                 * Lấy thông tin của người chơi đang tồn tại thông qua biến đối tượng peopleTank
                 * Và sau đó làm tròn tọa độ người chơi.
                */
                var peopleTank = MainForm.tanks[i];
                int peopleX = roundBlock(peopleTank.x);
                int peopleY = roundBlock(peopleTank.y);


                /*
                 * MainForm.peopleAttack[blockx, blocky] == i có nghĩa là nếu tọa độ nó đang đứng trong vùng nguy hiểm
                   và vùng nguy hiểm đó chính người chơi i tạo ra thì nó mới xem xét hướng để quyết định bắn.
                 
                 * Nói sơ qua mảng MainForm.peopleAttack:
                   - Nhiệm vụ của nó là liệt kê ra các vùng nguy hiểm do người chơi i tạo ra:
                   - Giá trị là 0 nếu người chơi 0 tạo ra (Tức là do người chơi P1 tạo ra)
                   - Giá trị là 1 nếu người chơi 1 tạo ra (Trong trường hợp 2 người chơi mới có giá trị này, tức là người chơi P2)
                   - Giá trị là 2 nếu đó là vùng giao thoa giữa 2 người chơi tạo ra.
                 
                 * Phép so sánh "Math.Abs(blockx * 16 - x) < deltaTime && Math.Abs(blocky * 16 - y) < deltaTime" có ý nghĩa gì:
                   - Ở đây: (blockx, blocky) là tọa độ nguyên kiểu ô vuông.
                            (x, y) là tọa độ số thực.
                   - Ta biết deltaTime là độ dịch chuyển pixel theo thời gian cho nên x hoặc y sẽ chứa bội số của deltaTime
                     -> Nếu xe tăng quyết định đưa ra hướng bắn khi nó nằm đúng tọa độ ô vuông nào đó, tức là nó phải nằm gọn
                        vào một ô vuông.
                   
                   - Ta biết blockx, blocky được tính theo dạng làm tròn.
                   -> Ở đây ta xét xem phép làm tròn thành số nguyên trên được coi là quá lố so với giá trị thực của nó hay không.
                      Cho nên nếu phép so sánh trên đúng thì nó làm tròn ở mức chấp nhận được.
                */
                if (MainForm.peopleAttack[blockx, blocky] == i && Math.Abs(blockx * 16 - x) < deltaTime && Math.Abs(blocky * 16 - y) < deltaTime)
                {
                    /*
                     * Nếu người và máy có tọa độ y bằng nhau thì nó xem xét đang nằm phía bên trái hay bên phải để đưa ra hướng bắn.
                    */
                    if (peopleY == roundBlock(y))
                    {
                        if (peopleX < roundBlock(x))
                        {
                            this.currentDir = Global.left;
                            this.shot();
                            return true;
                        }
                        if (peopleX > roundBlock(x))
                        {
                            this.currentDir = Global.right;
                            this.shot();
                            return true;
                        }
                    }
                    /*
                     * Nếu người và máy có tọa độ x bằng nhau thì nó xem xét đang nằm phía lên hay xuống để đưa ra hướng bắn.
                    */
                    if (peopleX == roundBlock(x))
                    {
                        if (peopleY < roundBlock(y))
                        {
                            this.currentDir = Global.up;
                            this.shot();
                            return true;
                        }
                        if (peopleY > roundBlock(y))
                        {
                            this.currentDir = Global.down;
                            this.shot();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /* TankAI - Note 9 */
        public override void Update()
        {
            /* Hàm preprocessorThink() được nói ở mục TankAI - Note 7 */
            preprocessorThink();

            /* Hàm lookOver() được nói ở mục TankAI - Note 8 */
            lookOver();

            /*
             * Ở đây nó xem xét thử nhà có thể bị xuyên phá hay không và đưa ra mục tiêu tấn công là xe tăng của người
               hay là nhà chính
            */
            if (!MainForm.homeIsSteel)
            {
                /* TankAI - Note 5 */
                /*
                 * Hàm checkAreaDown(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công "trực diện"  nhà mình
                   hay không.
                */
                if (checkAreaDown(blockx, blocky) && Math.Abs(blockx * 16 - x) < deltaTime && Math.Abs(blocky * 16 - y) < deltaTime)
                {
                    /*
                     * Nếu nó đang vùng có thể tấn công nhà chính nhưng mà nó không bị nguy hiểm thị quyết định bắn
                    */
                    if (MainForm.peopleAttack[blockx, blocky] == -1)
                    {
                        this.currentDir = Global.down;
                        this.shot();
                    }
                    /*
                     * Nếu nó đang vùng có thể tấn công nhà chính nhưng bị nguy hiểm thì phải tìm kiếm đường đi khác
                    */
                    else
                    {
                        findPath(0);
                    }
                }

                /* TankAI - Note 3 */
                /*
                 * Hàm checkAreaLeft(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công bên "trái" nhà mình
                   hay không.
                */
                else if (checkAreaLeft(blockx, blocky) && Math.Abs(blockx * 16 - x) < deltaTime && Math.Abs(blocky * 16 - y) < deltaTime)
                {
                    /*
                     * Nếu nó đang vùng có thể tấn công nhà chính nhưng mà nó không bị nguy hiểm thị quyết định bắn
                    */
                    if (MainForm.peopleAttack[blockx, blocky] == -1)
                    {
                        this.currentDir = Global.right;
                        this.shot();
                    }
                    /*
                    * Nếu nó đang vùng có thể tấn công nhà chính nhưng bị nguy hiểm thì phải tìm kiếm đường đi khác
                    */
                    else
                    {
                        findPath(0);
                    }
                }
                /* TankAI - Note 4 */
                /*
                 * Hàm checkAreaRight(int qx, int qy) nó sẽ cho chúng ta biết là xe tăng AI có đang nằm trong vùng tấn công bên "phải" nhà mình
                   hay không.
                */
                else if (checkAreaRight(blockx, blocky) && Math.Abs(blockx * 16 - x) < deltaTime && Math.Abs(blocky * 16 - y) < deltaTime)
                {
                    /*
                     * Nếu nó đang vùng có thể tấn công nhà chính nhưng mà nó không bị nguy hiểm thị quyết định bắn
                    */
                    if (MainForm.peopleAttack[blockx, blocky] == -1)
                    {
                        this.currentDir = Global.left;
                        this.shot();
                    }
                    /*
                    * Nếu nó đang vùng có thể tấn công nhà chính nhưng bị nguy hiểm thì phải tìm kiếm đường đi khác
                    */
                    else
                    {
                        findPath(0);
                    }
                }
                /*
                * Nếu nó không nằm trong vùng tấn công nào trong 3 vùng trên thì nó phải tìm kiếm đường đi tấn công nhà chính
                */
                else
                {
                    findPath(0);
                }
            }
            else
            /*
            * Nếu nhà chính đang được bảo vệ thì nó sẽ đi tìm đường đi tấn công người chơi.
            */
            {
                findPath(1);
            }
            base.Update();
        }

        /*
         * Hàm findPath(int areaTarget) này có lẽ được coi là hàm thực thi quan trọng nhất của bài toán này!
         * Nhiệm vụ của hàm findPath tùy giá trị của areaTarget được truyền vào mà có hai hướng xử lý như sau:
           - Trường hợp areaTarget = 0: Ta quy ước sẽ tìm đường đi từ điểm hiện hành của tăng máy đến một trong 3 vùng mà nó 
             có thể tấn công nhà chính một cách có thể coi là hợp lí nhất.
             + Ở mục lý thuyết người ta chỉ bảo tìm đường đi cho một điểm ban đầu và một điểm kết thúc, nhưng ở đây theo tôi nghĩ
               chúng ta có thể suy từ bài toán "tìm đường đi cho một điểm ban đầu và một điểm kết thúc" thành bài toán "tìm đường đi từ một điểm
               ban đầu đến nhiều điểm kết thúc"
             -> Suy ra ở đây cho cùng là khi tọa độ trong quá trình phát triển đỉnh mở rộng nó thỏa mãn một tính chất nào đó thì chúng ta
                sẽ dừng quá trình tìm kiếm và đưa ra đáp án.
           - Trường hợp areaTarget = 1: Ta quy ước sẽ tìm đường đi từ điểm hiện hành đến vùng mà nó có thể bắn được người chơi
             có thể là người chơi 1 hoặc người chơi 2, người chơi nào nó tìm ra trước thì nó sẽ bắn người đó.
             + Cốt lõi cho cùng cũng như nói ở trên, nó sẽ dừng khi tọa độ đỉnh chọn phát triển nó thỏa tính chất là nằm trong vùng
               có thể bắn được người chơi.
        */

        public bool findPath(int areaTarget)
        {
            /* -------> Tính tọa độ nguyên hiện hành của xe tăng máy -------> */

            int fromx = roundBlock(this.x);
            int fromy = roundBlock(this.y);

            /* <------- Tính tọa độ nguyên hiện hành của xe tăng máy <------- */



            List<Point> path      = new List<Point>();      // Danh sách này được khai báo để ghi lại đường đi đã tìm được
            BinaryHeap<Node> open = new BinaryHeap<Node>(); // BinaryHeap open này được khai báo các đỉnh chờ được chọn mở rộng tìm kiếm
            bool flag             = false;                  // Đánh dấu là đã tìm ra mục tiêu
            int[] dirx            = { 1, 0, -1, 0 };        // Độ tăng giá trị của x tùy vào hướng 0, 1, 2, 3 (UP, DOWN, LEFT, RIGHT)
            int[] diry            = { 0, 1, 0, -1 };        // Độ tăng giá trị của y tùy vào hướng 0, 1, 2, 3 (UP, DOWN, LEFT, RIGHT)
            Point target;                                   // Điểm sẽ di chuyển tiếp theo sau khi tìm kiếm, điểm này sẽ kề với (fromx, fromy)
            Node p;                                         // Node này sẽ dùng để lưu Node sau khi lấy từ tập open
            int qx, qy;                                     // Biểu thị tọa độ kề của p.x, và p.y
            int f;                                          // Biến dùng để tính giá trị ưu tiên của node được mở rộng

            /*
             * Mảng int visited này được khai báo mục đính để đánh dấu các điểm đã nằm trong tập open trong quá trình thực hiện bài toán.
            */
            int[,] visited = new int[25, 25];
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    visited[i, j] = 0;
                }
            }

            visited[fromx, fromy] = 1;                      // Đánh dấu đã đi qua đỉnh (fromx, fromy)
            open.Push(new Node(fromx, fromy, 0, null));     // Thêm đỉnh khởi tạo vào tập chứa open

            /*
             * Trong khi tập chứa open vẫn còn các đỉnh chưa được mở rộng. 
            */
            while (open.Count() != 0)
            {
                /*
                 * Lấy phần tử có tiềm năng nhất (tức là f nhỏ nhất) trong open ra
                */
                p = open.Pop();

                /*
                 * Tùy vào mục tiêu tìm kiếm mà nó ràng buộc điều kiện kết thúc của quá trình tìm kiếm
                   - Trường hợp areaTarget = 0: Ta quy ước sẽ tìm đường đi từ điểm hiện hành của tăng máy đến một trong 3 vùng mà nó 
                     tìm thấy đầu tiên.
                   - Trường hợp areaTarget = 1: Ta quy ước sẽ tìm đường đi từ điểm hiện hành đến vùng mà nó tìm thấy trước
                */
                switch (areaTarget)
                {
                    case 0:
                        if (checkAreaLeft(p.x, p.y) || checkAreaRight(p.x, p.y) || checkAreaDown(p.x, p.y) && MainForm.peopleAttack[p.x, p.y] == -1)
                        {
                            flag = true;
                        }
                        break;
                    case 1:
                        if (MainForm.peopleAttack[p.x, p.y] == 0 || MainForm.peopleAttack[p.x, p.y] == 1)
                        {
                            flag = true;
                        }
                        break;
                }

                /*
                 * Nếu nó tìm thấy mục tiêu, thì nó sẽ tìm điểm kế tiếp mà nó sẽ đi
                */
                if (flag)
                {
                    path.Add(new Point(fromx, fromy));

                    while (p != null)
                    {
                        path.Add(new Point(p.x, p.y));
                        p = p.pre;
                    }
                    pathss = path;

                    target = path[path.Count - 2];

                    if (fromx == target.X && target.Y == fromy - 1)
                    {
                        this.currentDir = Global.up;
                    }
                    else if (fromx == target.X && target.Y == fromy + 1)
                    {
                        this.currentDir = Global.down;
                    }
                    else if (fromy == target.Y && target.X == fromx - 1)
                    {
                        this.currentDir = Global.left;
                    }
                    else if (fromy == target.Y && target.X == fromx + 1)
                    {
                        this.currentDir = Global.right;
                    }

                    if (!this.move(this.currentDir))
                    {
                        this.shot();
                    }

                    return true;
                }
                
                for (int i = 0; i < 4; i++)
                {
                    qx = p.x + dirx[i];
                    qy = p.y + diry[i];
                    if (qx < 0 || qx > 24 || qy < 0 || qy > 24 || impediment[qx, qy] == 0 || MainForm.peopleAttack[qx, qy] == 2 || visited[qx, qy] == 1) 
                    {
                        continue; 
                    }

                    f = p.f + impediment[qx, qy];

                    if (MainForm.bulletsDanger[qx, qy] > -1)
                    {
                        f += 49;
                    }

                    if (MainForm.peopleAttack[qx, qy] == 0 || MainForm.peopleAttack[qx, qy] == 1 )
                    {
                        if (MainForm.peopleAttack[qx, qy] == 0)
                        {
                            if (MainForm.tanks[0].isShot)
                            {
                                //f += 449;
                            }
                            else
                            {
                                //f += 299;                            
                            }
                        }
                        if (MainForm.peopleAttack[qx, qy] == 1)
                        {
                            if (MainForm.tanks[1].isShot)
                            {
                                //f += 449;
                            }
                            else
                            {
                                //f += 299;                            
                            }
                        }
                    }
                    
                    open.Push(new Node(qx, qy, f, p));
                    visited[qx, qy] = 1;
                }
            }
            return false;
        }

        public override void Draw(Graphics canvas)
        {
            foreach(var item in pathss)
            {
                canvas.ResetTransform();
                canvas.TranslateTransform(item.X * 16, item.Y * 16);
                canvas.FillRectangle(Brushes.Red, 0, 0, 16, 16);
            }
            base.Draw(canvas);
        } 
    }
}
