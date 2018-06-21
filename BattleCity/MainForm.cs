using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace BattleCity
{
    public partial class MainForm : Form
    {
        // 
        // Embed Font
        // 
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbfont, uint cbfont, IntPtr pdv, [In] ref uint pcFonts);
        FontFamily fontFamily;
        public static Font fontLarge, fontSmall;

        /*-------------------------------------------> Các biến chính để giải quyết bài toán ---------------------------------------->*/

        public static List<Tank>      tanks;        // Danh sách thuộc kiểu lớp Tank để chứa các xe tăng đang tồn tại.
        public static List<Bullet>    bullets;      // Danh sách thuộc kiểu lớp Bullet để chứa các xe tăng đang tồn tại.
        public static List<Explosion> explosions;   // Danh sách thuộc kiểu lớp Exposion để chứa các vụ nổ đang tồn tại.
        public static List<Item>      items;        // Danh sách thuộc kiểu lớp Item để chứa các vật phẩm đang tồn tại.
        public static List<int>       awayTank;     // Danh sách thuộc kiểu biến int để chứa các kiểu xe tăng của máy.
        public static TankP1          P1Tank;       // Biến thuộc kiểu lớp TankP1 để chứa thông tin xe tăng người chơi một.
        public static TankP2          P2Tank;       // Biến thuộc kiểu lớp TankP2 để chứa thông tin xe tăng người chơi hai.
        public static TankAI          AITank;       // Biến thuộc kiểu lớp TankAI để tạo đối tượng mới thông qua biến này.
        public static Item            item;         // Biến thuộc kiểu lớp Item để tạo đối tượng vật phẩm mới thông qua biến này.
        public static int [,]         peopleAttack; // Mảng chức kiểu int để biểu thị vùng nguy hiểm mà người chơi có thể tạo ra.
        public static int [,]         bulletsDanger;// Mảng chức kiểu int để biểu thị vùng nguy hiểm mà đạn có thể tạo ra.
        public static int [,]         map;          // Mảng chứa kiểu int để biểu thị bản đồ ở hiện tại.

        /*<------------------------------------------- Các biến chính để giải quyết bài toán <----------------------------------------*/

        /*--------------------------------> Các biến liên quan đến phần xử lý giao diện tương tác người chơi------------------------->*/

        public int  gameStatus;                     // Biến biểu diễn các trạng thái giao diện của trò chơi.
        /*
         * Những giá trị gameStatus có thể nhận được:
           + gameStatus = 0: Ứng với trạng thái là giao diện menu của trò chơi (gameMenuUpdate).
           + gameStatus = 1: Ứng với trạng thái là giao diện trò chơi chính, lúc đang vào game (gamePlayUpdate).
           + gameStatus = 2: Ứng với trạng thái là giao diện trợ giúp của trò chơi (gameHelpUpdate).
           + gameStatus = 3: Ứng với trạng thái là giao diện giới thiệu của trò chơi (gameIntroductionUpdate).
           + gameStatus = 4: Ứng với trạng thái là giao diện thoát khỏi trò chơi (gameExit).
           + gameStatus = 5: Ứng với trạng thái là giao diện khi chúng ta chơi thua trò chơi (gameOver).
         */

        public int  choiceMenu;                     // Biến biểu diễn cho sự lựa chọn cho menu nào đó.
        /*
         * Những giá trị choiceMenu biễu diễn gồm:
           + choiceMenu = 0 : Sự lựa chọn để vào giao diện trò chơi dành cho 1 người chơi.
           + choiceMenu = 1 : Sự lựa chọn để vào giao diện trò chơi dành cho 2 người chơi.
           + choiceMenu = 2 : Sự lựa chọn để vào giao diện trợ giúp người chơi.
           + choiceMenu = 3 : Sự lựa chọn để vào giao diện giới thiệu trò chơi.
           + choiceMenu = 4 : Sự lựa chọn để vào giao diện kết thúc trò chơi.
        */

        public int  timeChoice;                     // Biến thời gian dao động tăng liên tục.
        public int  timeChoiceSize;                 // Khoảng thời gian để làm delay lúc ấn phím lựa chọn.
        public bool gamePause;                      // Biến đánh dấu trò chơi đang diễn ra hay đang ở trạng thái dừng.
        public int  timeExit;                       // Biến thời gian dao động tăng liên tục.

        /*<-------------------------------- Các biến liên quan đến phần xử lý giao diện tương tác người chơi<-------------------------*/

        /*------------------------------------>   Các biến liên quan đến trạng thái, xử lý sự kiện  ------------------------------->  */

        public static bool twoPlayer;               // Đánh dấu biểu thị chế độ dành cho 2 người chơi hay 1 người chơi.
        public static int  numberPeopleLive;        // Cho biết số người chơi còn tồn tại ở thời điểm hiện hành.
        public static bool P1Live;                  // Biểu thị sự tồn tại của người chơi thứ nhất.
        public static bool P2Live;                  // Biểu thị sự tồn tại của người chơi thứ hai.
        public static bool HomeLive;                // Biểu thị trạng thái của nhà mình.
        public static int  level;                   // Đánh dấu level hiện hành.
        public static int  timeInitTank;            // Biến thời gian dao động để sinh ra tăng cho máy khi đạt đến chu kỳ.
        public static int  timeInitTankSize;        // Chu kỳ thời gian để sinh ra tăng cho máy.
        public static bool homeIsSteel;             // Biểu thị nhà mình có khả năng chống đạn hay không.
        public static int  timeHome;                // Biến thời gian dao động để làm nhà mình hết thời gian chống đạn.
        public static int  timeHomeSize;            // Chy kỳ thời gian cho nhà mình chống đạn.
        public static int  xAttackLeft;             // Biên tấn công bên trái của tăng máy có thể bắn nhà mình chết.
        public static int  xAttackRight;            // Biên tấn công bên phải của tăng máy có thể bắn nhà mình chết.
        public static int  yAttackDown;             // Biên tấn công ở giữa của tăng máy có thể bắn chết nhà mình.
        public static bool debugMode;               // Đánh dấu trò chơi có ở chế độ debug lỗi hay không.
        public static Random rnd = new Random();    // Biến random

        
        /*------------------------------------>   Các biến liên quan đến trạng thái, xử lý sự kiện  ------------------------------->  */

        
        /*
         * Khởi tạo các giá trị ban đầu cho chương trình
         */

        public MainForm()
        {
            InitializeComponent();
            level            = 1;                 // Cho biết level trò chơi ban đầu là level 1
            gameStatus       = 0;                 // Cho biết trạng giao diện trò chơi ban đầu là giao diện menu
            HomeLive         = true;              // Cho biết ban đầu nhà ta còn sống
            map              = new int[26, 26];   // Cấp phát ô nhớ cho mảng map
            awayTank         = new List<int>();   // Khởi tạo danh sách kiểu int cho danh sách awayTank
            peopleAttack     = new int[25, 25];   // Cấp phát ô nhớ cho mảng peopleAttack
            bulletsDanger    = new int[25, 25];   // Câp phát ô nhớ cho mảng bulletsDanger
            choiceMenu       = 0;                 // Để con trỏ menu ban đầu ở vị trí đầu tiên trên cùng
            timeChoiceSize   = 7;                 // Thời gian delay chọn phím là 7/100 giây
            timeChoice       = 0;                 // Gắn cho biến thời gian để delay phím là 0
            timeExit         = 0;                 // Gắn cho biến thời gian để thoát game là 0
            timeInitTank     = 0;                 // Gắn cho biến thời gian để sinh ra tăng random cho máy là 0
            timeInitTankSize = 1000;              // Cứ sau khoảng thời gian 1000/100 giây thì sinh ra tăng cho máy
            timeHomeSize     = 1000;              // Cứ sau khoảng thời gian 1000/100 giây là khoảng thời gian chống đạn cho nhà
            debugMode        = true;              // Chế độ debug được bật hay tắt
        }

        /*
         * Tạo dựng các biến đối tượng, danh sách cần thiết cho một trò chơi mới.
        */

        private void newGame()
        {

           
            gamePause = false; // Đánh dấu trạng thái game đang chơi.

            /*
             * Mục tiêu của 2 vòng for ( ; ; ) này là khởi tạp bản đồ ban đầu cho trò chơi.
            */
            int[,] tempMap = new int[26, 26];
            switch (level)
            { 
                case 1:
                    tempMap = Global.map1;
                    break;
                case 2:
                    tempMap = Global.map2;
                    break;
                case 3:
                    //tempMap = Global.map3;
                    break;
                case 4:
                    //tempMap = Global.map4;
                    break;
                case 5:
                    //tempMap = Global.map5;
                    break;
                case 6:
                    //tempMap = Global.map6;
                    break;
                case 7:
                    //tempMap = Global.map7;
                    break;
            }

            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    map[i, j] = tempMap[i, j];
                }
            }


                /*------ Tạo danh sách mới cho các danh sách chứa các đối tượng như tanks, bullets, explosions*/
            tanks = new List<Tank>();
            bullets = new List<Bullet>();
            explosions = new List<Explosion>();
            items = new List<Item>();
            /*------ Tạo danh sách mới cho các danh sách chứa các đối tượng như tanks, bullets, explosions*/

            HomeLive = true;     // Đánh dấu cho biết nhà mình đang còn tồn tại.
            homeIsSteel = false; // Đánh dấu cho biết trạng thái nhà mình không thể xuyên phá là sai.

            P1Tank = new TankP1(); // Khởi tạo đối tượng xe tăng cho người chơi 1.
            P1Live = true; // Đánh dấu cho biết người chơi 1 đang còn sống.
            P2Live = false; // Đánh dấu cho biết người chơi 2 đã chết.
            numberPeopleLive = 1; // Đánh dấu cho biết chế độ chơi hiện tại là 1 người chơi.

            if (twoPlayer) // Nếu chế độ hiện tại là hai người chơi thì khởi tạo các thông tin cho người chơi 2.
            {
                /*
                 * Mục tiêu của vùng code này là khởi tạo mảng chứa các phần tử ngẫu nhiên [2, 4] để tạo ra các xe tăng cho máy.
                */
                awayTank.Clear(); // Làm sạch danh sách.
                for (int i = 0; i <= 10; i++)
                {
                    awayTank.Add(randomNumber(2, 4)); // Thêm phần tử vào danh sách.
                }
                numberPeopleLive++; // Số lượng người chơi còn sống lúc ban đầu đang là 2.s
                P2Tank = new TankP2(); // Khởi tạo đối tượng xe tăng cho người chơi 2.
                P2Live = true; // Đánh dấu cho biết người chơi 2 đang còn sống.
            }
            else
            {
            /*
             * Mục tiêu của vùng code này là khởi tạo mảng chứa các phần tử ngẫu nhiên [2, 4] để tạo ra các xe tăng cho máy.
            */
                awayTank.Clear(); // Làm sạch danh sách.
                for (int i = 0; i <= 5; i++)
                {
                    awayTank.Add(randomNumber(2, 4)); // Thêm phần tử vào danh sách.
                }
            }
           
            timeInitTank = timeInitTankSize - 1;

            int temp;
            xAttackRight = xAttackLeft = yAttackDown = -1;
            for (temp = 9; temp > -1; temp--)
            {
                if (check4block16(temp, 24) == 0)
                {
                    xAttackLeft = temp + 1;
                    break;
                }
            }
            if (xAttackLeft == -1)
            {
                xAttackLeft = temp + 1;
            }
            for (temp = 15; temp < 25; temp++)
            {
                if (check4block16(temp, 24) == 0)
                {
                    xAttackRight = temp - 1;
                    break;
                }
            }
            if (xAttackRight == -1)
            {
                xAttackRight = temp - 1;
            }
            for (temp = 21; temp > -1; temp--)
            {
                if (check4block16(12, temp) == 0)
                {
                    yAttackDown = temp + 1;
                    break;
                }
            }
            if (yAttackDown == -1)
            {
                yAttackDown = temp + 1;
            }

            
        }

        /*
         * Đây là hàm quan trọng bậc nhất trong thuật giải A* của bài toán này, nó sẽ giúp các xe tăng của máy có thể ước lượng
           hướng di chuyển hiệu quả (Đôi khi xác suất không phải là 100% bởi vì ước lượng dựa trên suy đoán của con người).
             
         * Những nhiệm vụ chính của hàm preprocessorAI() được coi như là hàm tiền xử lý, quan sát nhận ra những trở ngại
           để quyết định lúc nào các xe tăng nên bắn đạn để có lợi cho bản thân nó.
             
         * Những mục tiêu có thể gây ra trờ ngại cho xe tăng của máy đó chính là:
           + Nếu nó nằm trong tầm ngắm của người chơi, thì tất nhiên là người chơi sẽ nằm trong tầm ngắm của nó
            -> Nó sẽ quyết định bắn lúc này nếu có thể bắn nếu nó nằm trong vùng này, và nó nhận ra đường đi vào vùng này sẽ nguy hiểm.
           + Nếu có một viên đạn nào đó hướng vào nó thì thì có thể ảnh hướng tới tính mạng.
            -> Nó sẽ quyết định bắn để tự thủ nếu nó nằm trong vùng này, không thì nó sẽ tránh vùng này ra.
        */

        private void preprocessorAI()
        {

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    bulletsDanger[i, j] = -1;
                }
            }

            foreach (var item in bullets.ToList())
            {
                if (!(item.typeBullet == 0 || item.typeBullet == 1))
                {
                    continue;
                }
                int BulletX = (int)(item.x - 12) / 16;
                int BulletY = (int)(item.y - 12) / 16;
                int blockx, blocky;
                switch (item.dir)
                {
                    case Global.up:
                        for (blocky = BulletY; blocky > -1; blocky--)
                        {
                            if (check4block16(BulletX, blocky) == 0)
                            {
                                break;
                            }
                            if (checkBullet(map[blocky, BulletX], map[blocky, BulletX + 1]))
                            {
                                bulletsDanger[BulletX, blocky] = Global.down;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case Global.down:
                        for (blocky = BulletY; blocky < 25; blocky++)
                        {
                            if (check4block16(BulletX, blocky) == 0)
                            {
                                break;
                            }
                            if (checkBullet(map[blocky, BulletX], map[blocky, BulletX + 1]))
                            {
                                bulletsDanger[BulletX, blocky] = Global.up;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case Global.left:
                        for (blockx = BulletX; blockx > -1; blockx--)
                        {
                            if (check4block16(blockx, BulletY) == 0)
                            {
                                break;
                            }
                            if (checkBullet(map[BulletY, blockx], map[BulletY + 1, blockx]))
                            {
                                bulletsDanger[blockx, BulletY] = Global.right;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    case Global.right:
                        for (blockx = BulletX + 1; blockx < 25; blockx++)
                        {
                            if (check4block16(blockx, BulletY) == 0)
                            {
                                break;
                            }
                            if (checkBullet(map[BulletY, blockx], map[BulletY + 1, blockx]))
                            {
                                bulletsDanger[blockx, BulletY] = Global.left;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                }
            }

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    peopleAttack[i, j] = -1;
                }
            }
            for (int i = 0; i < numberPeopleLive; i++)
            {
                var People = tanks[i];
                int PeopleX = roundBlock(People.x);
                int PeopleY = roundBlock(People.y);
                int blockx, blocky;
                for (blocky = PeopleY - 1; blocky > -1; blocky--)
                {
                    if (check4block16(PeopleX, blocky) == 0)
                    {
                        break;
                    }
                    if (checkBullet(map[blocky, PeopleX], map[blocky, PeopleX + 1]))
                    {
                        if (peopleAttack[PeopleX, blocky] > -1)
                        {
                            peopleAttack[PeopleX, blocky] = 2;
                        }
                        else
                        {
                            peopleAttack[PeopleX, blocky] = i;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (blocky = PeopleY + 1; blocky < 25; blocky++)
                {
                    if (check4block16(PeopleX, blocky) == 0)
                    {
                        break;
                    }
                    if (checkBullet(map[blocky, PeopleX], map[blocky, PeopleX + 1]))
                    {
                        if (check4block16(PeopleX, blocky) != 0)
                        {
                            if (peopleAttack[PeopleX, blocky] > -1)
                            {
                                peopleAttack[PeopleX, blocky] = 2;
                            }
                            else
                            {
                                peopleAttack[PeopleX, blocky] = i;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (blockx = PeopleX - 1; blockx > -1; blockx--)
                {
                    if (check4block16(blockx, PeopleY) == 0)
                    {
                        break;
                    }
                    if (checkBullet(map[PeopleY, blockx], map[PeopleY + 1, blockx]))
                    {
                        if (check4block16(blockx, PeopleY) != 0)
                        {
                            if (peopleAttack[blockx, PeopleY] > -1)
                            {
                                peopleAttack[blockx, PeopleY] = 2;
                            }
                            else
                            {
                                peopleAttack[blockx, PeopleY] = i;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                for (blockx = PeopleX + 1; blockx < 25; blockx++)
                {
                    if (check4block16(blockx, PeopleY) == 0)
                    {
                        break;
                    }
                    if (checkBullet(map[PeopleY, blockx], map[PeopleY + 1, blockx]))
                    {
                        if (check4block16(blockx, PeopleY) != 0)
                        {
                            if (peopleAttack[blockx, PeopleY] > -1)
                            {
                                peopleAttack[blockx, PeopleY] = 2;
                            }
                            else
                            {
                                peopleAttack[blockx, PeopleY] = i;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /* -------------------------------------------->  Update Method Code Area  -------------------------------------------> */

        /* 
         * Cứ sau mỗi khoảng thời gian là 1/1000 giây (Interval = 1) thì MainForm nó sẽ gọi đến hàm Update(object sender, EventArgs e) (Vì 
           ở sự kiện Tick của gameTimer đã thêm EventHandler là hàm Update(...) này).
         
         * Mục đích của hàm Update này điểu khiển các hàm Update con tùy vào những trạng thái trò chơi khác nhau gồm các trạng thái (
           gameStatus) như sau:
           + gameStatus = 0: Ứng với trạng thái là giao diện menu của trò chơi (gameMenuUpdate).
           + gameStatus = 1: Ứng với trạng thái là giao diện trò chơi chính, lúc đang vào game (gamePlayUpdate).
           + gameStatus = 2: Ứng với trạng thái là giao diện trợ giúp của trò chơi (gameHelpUpdate).
           + gameStatus = 3: Ứng với trạng thái là giao diện giới thiệu của trò chơi (gameIntroductionUpdate).
           + gameStatus = 4: Ứng với trạng thái là giao diện thoát khỏi trò chơi (gameExit).
           + gameStatus = 5: Ứng với trạng thái là giao diện khi chúng ta chơi thua trò chơi (gameOver).
         
         * Tùy vào giá trị của gameStatus tức là chúng ta nên cập nhật những biến nào trong chương trình thì nó sẽ gọi các hàm Update
           con ở trên.
        */

        private void Update(object sender, EventArgs e)
        {
            switch (gameStatus)
            {
                case 0:
                    gameMenuUpdate();
                    break;
                case 1:
                    gamePlayUpdate();
                    break;
                case 2:
                    gameHelpUpdate();
                    break;
                case 3:
                    gameIntroductionUpdate();
                    break;
                case 4:
                    gameExit();
                    break;
                case 5:
                    gameOver();
                    break;
            }
        }

        /*
         * Mục đính của hàm gameMenuUpdate() là cập nhật biến điều khiển các lựa chọn của menu game.
         
         * Biến điều khiển của Menu được tạo có tên là choiceMenu, nó có các giá trị như sau.
           + choiceMenu = 0 : Sự lựa chọn để vào giao diện trò chơi dành cho 1 người chơi.
           + choiceMenu = 1 : Sự lựa chọn để vào giao diện trò chơi dành cho 2 người chơi.
           + choiceMenu = 2 : Sự lựa chọn để vào giao diện trợ giúp người chơi.
           + choiceMenu = 3 : Sự lựa chọn để vào giao diện giới thiệu trò chơi.
           + choiceMenu = 4 : Sự lựa chọn để vào giao diện kết thúc trò chơi.
        */

        private void gameMenuUpdate()
        {
            if (Keyboard.IsKeyDown(Keys.Enter))
            {
                switch (choiceMenu)
                {
                    case 0:
                        gameStatus = 1;    // Đánh dấu trạng thái của trò chơi là sẽ vào giao diện trò chơi chính.
                        twoPlayer = false; // Đánh dấu sẽ vào giao diện trò chơi với chế độ dành cho 1 người chơi.
                        newGame();
                        break;
                    case 1:
                        gameStatus = 1;    // Đánh dấu trạng thái của trò chơi là sẽ vào giao diện trò chơi chính.
                        twoPlayer = true;  // Đánh dấu sẽ vào giao diện trò chơi với chế độ dành cho 2 người chơi.
                        newGame();
                        break;
                    case 2:
                        gameStatus = 2;    // Đánh dấu trạng thái của trò chơi là sẽ vào giao diện trợ giúp người chơi.
                        break;
                    case 3:
                        gameStatus = 3;    // Đánh dấu trạng thái của trò chơi là sẽ vào giao diện giới thiệu trò chơi.
                        break;
                    case 4:
                        gameStatus = 4;    // Đánh dấu trạng thái của trò chơi là sẽ vào giao diện kết thúc trò chơi.
                        break;
                }
            }
            /*
             * Đây là điều kiện để làm delay khi chọn phím, tức là làm cho input điều khiển menu không nhảy liên tục mà nhảy
               sau khoảng thời gian của timeChoiceSize.
             * Có nghĩa là biến thời gian tăng từ 0 đến timeChoiceSize, nếu nó vượt quá thì đặt lại giá trị 0. 
            */
            if (timeChoice > timeChoiceSize)
            {
                /*
                 * Nếu đầu vào của phím nhận được là lên (UP) trên hoặc sang trái (LEFT) 
                   thì sẽ thay đổi giá trị của biến điều khiển menu.
                */
                if (Keyboard.IsKeyDown(Keys.Up) || Keyboard.IsKeyDown(Keys.Left))
                {
                    /*
                     * Nếu biến điều khiển ở vị trí đầu tiên thì cho nó xuống vị trí dưới cùng.
                    */
                    if (choiceMenu == 0)
                    {
                        choiceMenu = 4;
                    }
                    /*
                     * Dịch chuyển biến điều khiển lên trên một đơn vị.
                    */
                    else
                    {
                        choiceMenu--;
                    }
                }
                /*
                 * Nếu đầu vào của phím nhận được là lên (DOWN) trên hoặc sang trái (RIGHT)
                   thì sẽ thay đổi giá trị của biến điều khiển menu.
                */
                else if (Keyboard.IsKeyDown(Keys.Down) || Keyboard.IsKeyDown(Keys.Right))
                {
                    /*
                     * Nếu biến điều khiển ở dưới cùng thì cho nó xuống thì cho nó lên vị trí đầu tiên.
                    */
                    if (choiceMenu == 4)
                    {
                        choiceMenu = 0;
                    }
                    /*
                     * Dịch chuyển biến điều khiển xuống dưới một đơn vị.
                    */
                    else
                    {
                        choiceMenu++;
                    }
                }
                timeChoice = 0; // Đặt lại biến để xử lý delay đầu vào của phím.
            }
            timeChoice++;  // Tăng giá trị biến để xử lý delay đầu vào của phím.
            gamePanel.Invalidate();
        }

        /*
         * Mục đích của hàm gamePlayUpdate() là cập nhật các biến để xử lý các đối tượng tồn tại trong trò chơi, các đối tượng đó
           cụ thể là các đối tượng sau:
           + Xử lý các đối tượng "xe tăng" đăng tồn tại trong danh sách "tanks", danh sách "tanks" đang chứa các đối tượng là xe tăng
             đang hiện hành trong trò chơi (cập nhật vị trí & xử lý va chạm giữ tăng và chướng ngại vật, hành động bắn,...).
           + Xử lý các đối tượng "đạn" đăng tồn tại trong danh sách "bullets", danh sách "bullets" đang chứa các đối tượng là đạn
             đang hiện hành trong trò chơi (cập nhật vị trí & xử lý va chạm giữa đạn và tăng hoặc đạn và đạn,...).
           + Xử lý các đối tượng "vụ nổ" đăng tồn tại trong danh sách "explosions", danh sách "explosions" đang chứa các đối tượng là
             vụ nổ đang hiện hành trong trò chơi (cập nhật các fram ảnh cho chuyển động của vụ nổ).
           + Xử lý sự kiện sinh ra các xe tăng của máy sau mỗi khoảng thời gian cố định nào đó.
           + Xử lý trạng thái để kết thúc trò chơi.
           + Xử lý sự kiện nếu là trạng thái game đang dừng.
         * Đây có thể coi là hàm điều khiển, cập nhật quan trọng nhất trong trò chơi.
        */

        private void gamePlayUpdate()
        {
            if(numberPeopleLive > 0 && awayTank.Count == 0 && tanks.Count == numberPeopleLive)
            {
                level++;
                newGame();
                return;
            }
            if (homeIsSteel)
            {
                if (timeHome > timeHomeSize)
                {
                    MainForm.map[25, 11] = Global.wall;
                    MainForm.map[24, 11] = Global.wall;
                    MainForm.map[23, 11] = Global.wall;
                    MainForm.map[25, 14] = Global.wall;
                    MainForm.map[24, 14] = Global.wall;
                    MainForm.map[23, 14] = Global.wall;
                    MainForm.map[23, 12] = Global.wall;
                    MainForm.map[23, 13] = Global.wall;
                    timeHome = 0;
                    homeIsSteel = false;
                }
                else
                {
                    timeHome++;
                }
            }
            
            /*
             * Nếu trạng thái của trò chơi chính là trạn thái dừng, sẽ cập nhật các biến chọn menu ở trạng thái dừng.
            */
            if (gamePause)
            {
                if (Keyboard.IsKeyDown(Keys.Enter))
                {
                    switch (choiceMenu)
                    {
                        case 0:
                            gamePause = false; // Đánh dấu trạng thái sẽ tiếp tục trò chơi.
                            break;
                        case 1:
                            gameStatus = 0;    // Đánh dấu trạng thái sẽ vào giao diện menu trò chơi.
                            choiceMenu = -1;   // Đánh dấu -1 để khi vào giao diện menu trò chơi nó sẽ không nhận giá trị nào vào khởi tạo lại.
                            timeChoice = 0;    // Đặt lại biến để xử lý delay đầu vào của phím.
                            gamePause = true;  // Đánh dấu trạng thái sẽ tiếp tục trò chơi.
                            break;
                    }
                }
                /*
                 * Đây là điều kiện để làm delay khi chọn phím, tức là làm cho input điều khiển menu không nhảy liên tục mà nhảy
                   sau khoảng thời gian của timeChoiceSize.
                 * Có nghĩa là biến thời gian tăng từ 0 đến timeChoiceSize, nếu nó vượt quá thì đặt lại giá trị 0. 
                */
                if (timeChoice > timeChoiceSize)
                {
                    if (Keyboard.IsKeyDown(Keys.Up) || Keyboard.IsKeyDown(Keys.Left))
                    {
                        /*
                         * Nếu biến điều khiển ở vị trí đầu tiên thì cho nó xuống vị trí dưới cùng.
                        */
                        if (choiceMenu == 0)
                        {
                            choiceMenu = 1;
                        }
                        /*
                         * Dịch chuyển biến điều khiển lên trên một đơn vị.
                        */
                        else
                        {
                            choiceMenu--;
                        }
                    }
                    else if (Keyboard.IsKeyDown(Keys.Down) || Keyboard.IsKeyDown(Keys.Right))
                    {
                        /*
                         * Nếu biến điều khiển ở dưới cùng thì cho nó xuống thì cho nó lên vị trí đầu tiên.
                        */
                        if (choiceMenu == 1)
                        {
                            choiceMenu = 0;
                        }
                        /*
                         * Dịch chuyển biến điều khiển xuống dưới một đơn vị.
                        */
                        else
                        {
                            choiceMenu++;
                        }
                    }
                    timeChoice = 0; // Đặt lại biến để xử lý delay đầu vào của phím.
                }
                timeChoice++; // Tăng giá trị biến để xử lý delay đầu vào của phím.
                gamePanel.Invalidate();
                return;
            }

            /*
             * Đoạn này sẽ xử lý việc sinh ra đối tượng tăng của máy sau một khoảng thời gian cố định timeInitTankSize nào đó.
             
             * Nếu số lượng tăng của máy còn để sinh ra là điều kiện để sinh tăng của máy. (awayTank.Count > 0).
             
             * Nếu timeInitTank đạt ngưỡng khoảng thời gian timeInitTankSize (timeInitTank > timeInitTankSize)
               là điều kiện để sinh tăng của máy.
            */
            if (timeInitTank > timeInitTankSize && awayTank.Count > 0)
            {
                /*
                 * Tạo đối tượng mới với, với awayTank[0] là chủng loại tăng được sinh ra, tùy vào giá trị 
                   mà nó sẽ sinh ra loại tăng khác nhau
                 * Sau khi dùng xong awayTank[0] thì chúng ta sẽ xóa phần tử đó ra khỏi mảng awayTank.
                */
                for (int i = 0; i < numberPeopleLive; i++)
                {
                    item = new Item(randomNumber(0, 2), randomNumber(13, 24), randomNumber(13, 24));

                    AITank = new TankAI(awayTank[0]);
                    awayTank.RemoveAt(0);
                    timeInitTank = 0; // Đặt lại biến để xử lý cho việc khoảng thời gian sinh tăng.
                }
            }
            timeInitTank++; // Tăng giá trị biến để xử lý cho việc khoảng thời gian sinh tăng.

            /*
             * Đây là điều kiện để kết luận người chơi đã thua (Có thể có 2 người chơi và cả hai đã chết hết hoặc nhà đã bị giết).
            */
            if (!P1Live && !P2Live || !HomeLive)
            {
                gameStatus = 5;
                return;
            }

            /*
             * Nếu bức tường xung quang nhà được bảo vệ thì gán giá trị cho nó là những chứng ngại vật thuộc thể loại không thể 
               xuyên phá được (Global.steel).
            */
            if (homeIsSteel)
            {
                MainForm.map[25, 11] = Global.steel;
                MainForm.map[24, 11] = Global.steel;
                MainForm.map[23, 11] = Global.steel;
                MainForm.map[25, 14] = Global.steel;
                MainForm.map[24, 14] = Global.steel;
                MainForm.map[23, 14] = Global.steel;
                MainForm.map[23, 12] = Global.steel;
                MainForm.map[23, 13] = Global.steel;
            }


            /*
             * Ở đây, hai vòng for( ; ; ) này sẽ xử lý việc vụ việc các viên đạn có chạm nhau hay không.
             * Công việc này không thể xử lý nội trong đối tượng. Giả sử:
               Chúng ta đang nằm trong Class Bullet, khi nó truy vét tất cả đối tượng trong danh sách bullets, và 
               kèm theo đó nó sẽ kiểm tra va chạm các viên đạn xung quanh.
                - Khi nó phát hiện ra va chạm với "other" thì ngay tức khắc lúc đó đối tượng "other" cũng phát hiện ra
                  nó, ở đây chúng ta sẽ không biết là sẽ hủy đối tượng nào ra khỏi mảng trước, nếu xóa thì sẽ sinh lỗi ngay.
               -> Do đó chúng ta sẽ xét đến va chạm và để xóa hai viên đạn va chạm nhau một cách giống như là ở giữa hai đối tượng đó
                  cái nào có chỉ nằm trong danh sách bullets lớn hơn thì sẽ xóa nó trước, làm vậy để tránh lỗi nêu trên.
            */
            for (int i = 0; i < bullets.Count; i++)
            {
                for (int j = 0; j < bullets.Count; j++)
                {
                    if (i == j) { continue; }
                    else if (bullets[i].hitTest(bullets[j]) && bullets[i].typeBullet != bullets[j].typeBullet)
                    {
                        var explsion = new Explosion(0, bullets[i].x, bullets[i].y);
                        if (i > j) { bullets.Remove(bullets[i]); bullets.Remove(bullets[j]); }
                        else { bullets.Remove(bullets[j]); bullets.Remove(bullets[i]); }
                    }
                }
            }


            /*
             * Đây là hàm quan trọng bậc nhất trong thuật giải A* của bài toán này, nó sẽ giúp các xe tăng của máy có thể ước lượng
               hướng di chuyển hiệu quả (Đôi khi xác suất không phải là 100%).
             
             * Những nhiệm vụ chính của hàm preprocessorAI() được coi như là hàm tiền xử lý, quan sát nhận ra những trở ngại
               để quyết định lúc nào các xe tăng nên bắn đạn để có lợi cho bản thân nó.
             
             * Những mục tiêu có thể gây ra trờ ngại cho xe tăng của máy đó chính là:
               + Nếu nó nằm trong tầm ngắm của người chơi, thì tất nhiên là người chơi sẽ nằm trong tầm ngắm của nó
                 -> Nó sẽ quyết định bắn lúc này nếu có thể bắn nếu nó nằm trong vùng này, và nó nhận ra đường đi vào vùng này sẽ nguy hiểm.
               + Nếu có một viên đạn nào đó hướng vào nó thì thì có thể ảnh hướng tới tính mạng.
                 -> Nó sẽ quyết định bắn để tự thủ nếu nó nằm trong vùng này, không thì nó sẽ tránh vùng này ra.
            */
            preprocessorAI();

            /*
             * Từng vòng for ở dưới đây nó sẽ gọi đến phương thức cập nhật nội bội đối tượng
             
             * Các đối tượng cần phải cập nhật đó là:
               + Các xe tăng nằm trong danh sách bullets gồm cả người và máy.
               + Các viên đạn tồn tại trong thời gian hiện hành.
               + Các vụ nổ tồn tại trong thời gian hiện hành.
            */
            foreach (var item in tanks.ToList())
            {
                item.Update();
            }
            foreach (var item in bullets.ToList())
            {
                item.Update();
            }
            foreach (var item in explosions.ToList())
            {
                item.Update();
            }
            foreach (var item in items.ToList())
            {
                item.Update();
            }
            gamePanel.Invalidate();
        }

        /*
         * Hàm gameHelpUpdate() này đơn giản, nó chỉ việc nhận đầu vào bàn phím để quay về giao diện menu trò chơi.
         
         * Nhiệm vụ của nó là nhận đầu vào của phím Escape và đưa biến điều khiểu giao diện về menu trò chơi.
        */

        private void gameHelpUpdate()
        {
            if (Keyboard.IsKeyDown(Keys.Escape))
            {
                gameStatus = 0;
            }
        }

        /*
         * Hàm gameIntroductionUpdate() này đơn giản, nó chỉ việc nhận đầu vào bàn phím để quay về giao diện menu trò chơi.
         
         * Nhiệm vụ của nó là nhận đầu vào của phím Escape và đưa biến điều khiểu giao diện về menu trò chơi.
        */

        private void gameIntroductionUpdate()
        {
            if (Keyboard.IsKeyDown(Keys.Escape))
            {
                gameStatus = 0;
            }
            gamePanel.Invalidate();
        }

        /*
         * Hàm gameExit() này đơn giản, nó chỉ việc làm delay một thời gian để hiện giao diện thoát trò chơi
           rồi thoát khỏi trò chơi.
         * Nó sẽ thoát khỏi trò chơi khi timeExit vượt khoảng thời gian cố định.
        */

        private void gameExit()
        {
            if (timeExit > 300)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                timeExit++;
            }
        }

        /*
         * Hàm gameOver() này đơn giản, nó chỉ việc làm delay một thời gian để hiện giao diện khi chúng ta chơi thua
           rồi đưa giao diện trò chơi về menu trò chơi.
         * Nó sẽ đưa về menu trò chơi khi timeExit vượt khoảng thời gian cố định.
        */

        private void gameOver()
        {
            if (timeExit > 300)
            {
                timeExit = 0;
                gameStatus = 0;
            }
            else
            {
                timeExit++;
            }
            this.gamePanel.Invalidate();
        }

        /* <-------------------------------------------- Update Method Code Area <------------------------------------------- */


        /* --------------------------------------------->  Draw Method Code Area   -------------------------------------------> */

        /* 
         * Cứ sau mỗi khoảng thời gian là 1/1000 giây (Interval = 1) thì các một trong những hàm Update trong các hàm sau sẽ gọi đến
           phương thức gamePanel.Invalidate() của biến panel gamePanel:
           + gameMenuUpdate();
           + gamePlayUpdate();
           + gameHelpUpdate();
           + gameIntroductionUpdate();
           + gameExit();
           + gameOver();
           -> Do đó, vì hàm Draw đã được gắn PaintEventHandler vào cho panel gamePanel, nên nó sẽ được gọi sau khoảng thời gian
              1/1000 giây.
         
         * Mục đích của hàm Draw này điểu khiển các hàm Draw con tùy vào những trạng thái trò chơi khác nhau gồm các trạng thái (
           gameStatus) như sau:
           + gameStatus = 0: Ứng với trạng thái là giao diện menu của trò chơi (gameMenuUpdate).
           + gameStatus = 1: Ứng với trạng thái là giao diện trò chơi chính, lúc đang vào game (gamePlayUpdate).
           + gameStatus = 2: Ứng với trạng thái là giao diện trợ giúp của trò chơi (gameHelpUpdate).
           + gameStatus = 3: Ứng với trạng thái là giao diện giới thiệu của trò chơi (gameIntroductionUpdate).
           + gameStatus = 4: Ứng với trạng thái là giao diện thoát khỏi trò chơi (gameExit).
           + gameStatus = 5: Ứng với trạng thái là giao diện khi chúng ta chơi thua trò chơi (gameOver).
         
         * Tùy vào giá trị của gameStatus tức là chúng ta nên cập nhật những biến nào trong chương trình thì nó sẽ gọi các hàm Draw
           con ở trên, nhiệm vụ của nó là vẽ ra các trạng thái thời điểm nào đó của trò chơi.
        */

        private void Draw(object sender, PaintEventArgs e)
        {
            switch (gameStatus)
            {
                case 0:
                    gameMenuDraw(e.Graphics);
                    break;
                case 1:
                    gamePlayDraw(e.Graphics);
                    break;
                case 2:
                    gameHelpDraw(e.Graphics);
                    break;
                case 3:
                    gameIntroductionDraw(e.Graphics);
                    break;
                case 4:
                    gameExitDraw(e.Graphics);
                    break;
                case 5:
                    gameOverDraw(e.Graphics);
                    break;
            }
        }

        /*
         * Nhiệm vụ của hàm gameMenuDraw(Graphics canvas) là vẽ ra giao diện Menu của trò chơi.
        */

        private void gameMenuDraw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 480);

            canvas.DrawString("Battle City", fontLarge, Brushes.Chocolate, 40, 80);
            canvas.DrawString("PLAY", fontSmall, choiceMenu == 0 ? Brushes.Yellow : Brushes.WhiteSmoke, 167, 180);
            canvas.DrawString("2 PLAYERS", fontSmall, choiceMenu == 1 ? Brushes.Yellow : Brushes.WhiteSmoke, 130, 220);
            canvas.DrawString("INTRODUCTION", fontSmall, choiceMenu == 3 ? Brushes.Yellow : Brushes.WhiteSmoke, 115, 260);
            canvas.DrawString("EXIT", fontSmall, choiceMenu == 4 ? Brushes.Yellow : Brushes.WhiteSmoke, 167, 300);
        }


        /*
         * Mục đích của gamePlayDraw(Graphics canvas) là vẽ ra các đối tượng tồn tại trong trò chơi chính,
           đây được coi là giao diện trò chơi quan trọng nhất.
        */

        private void gamePlayDraw(Graphics canvas)
        {
            /* ------------------------> Background Code Area Draw Code ------------------------------->*/
            /*
             * Vẽ ra hình nền cho trò chơi là màu đen.
            */
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 416);

            if (gamePause) // Nếu giao diện trò chơi hiện tại là trạng thái dừng.
            {
                canvas.ResetTransform();
                canvas.DrawString("PAUSED", fontSmall, Brushes.Chocolate, 166, 160);
                canvas.DrawString("CONTINUE", fontSmall, choiceMenu == 0 ? Brushes.Yellow : Brushes.WhiteSmoke, 150, 200);
                canvas.DrawString("BACK TO MENU", fontSmall, choiceMenu == 1 ? Brushes.Yellow : Brushes.WhiteSmoke, 127, 240);

                canvas.ResetTransform();
                canvas.TranslateTransform(0, 416);
                canvas.FillRectangle(Brushes.Black, 0, 0, 416, 64);
                return;
            }

            /*
             * Vẽ ra hình nền cho khung xe tăng còn lại là màu xám.
            */
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 416);
            canvas.FillRectangle(Brushes.Gray, 0, 0, 416, 64);

            /*
             * Vẽ ra chữ AI màu cam, P1 và P2 nếu có là cam luôn.
            */
            canvas.ResetTransform();
            canvas.DrawString("AI", fontSmall, Brushes.OrangeRed, 0, 420);
            if (MainForm.P1Live)
            {
                canvas.ResetTransform();
                canvas.DrawString("P1", fontSmall, Brushes.OrangeRed, 0, 440);
            }
            if (MainForm.P2Live)
            {
                canvas.ResetTransform();
                canvas.DrawString("P2", fontSmall, Brushes.OrangeRed, 0, 460);
            }

            /*
             * Vẽ ra các chiếc xe tăng còn lại của xe tăng máy, của P1 và P2 nếu có.
            */
            for (int i = 0; i < awayTank.Count; i++)
            {
                canvas.ResetTransform();
                canvas.TranslateTransform(32 + i * 25, 416);
                canvas.DrawImage(Global.sprites, new Rectangle(0, 0, 20, 20), new Rectangle(awayTank[i] == 4 ? 6 * 64 : awayTank[i] * 64, 0, 32, 32), GraphicsUnit.Pixel);
            }

            if (MainForm.P1Live) // Nếu người chơi P1 có tồn tại trong trò chơi hiện hành.
            {
                for (int i = 0; i < P1Tank.life; i++)
                {
                    canvas.ResetTransform();
                    canvas.TranslateTransform(32 + i * 25, 436);
                    canvas.DrawImage(Global.sprites, new Rectangle(0, 0, 20, 20), new Rectangle(0, 0, 32, 32), GraphicsUnit.Pixel);
                }
            }

            if (MainForm.P2Live) // Nếu người chơi P2 có tồn tại trong trò chơi hiện hành.
            {
                for (int i = 0; i < P2Tank.life; i++)
                {
                    canvas.ResetTransform();
                    canvas.TranslateTransform(32 + i * 25, 456);
                    canvas.DrawImage(Global.sprites, new Rectangle(0, 0, 20, 20), new Rectangle(64, 0, 32, 32), GraphicsUnit.Pixel);
                }
            } 

            /* <------------------------------------- Background Code Area Draw Code <------------------------------------*/



            /* ------------------------------------->      Debug Mode Draw Code   ---------------------------------------->*/
            

            if (debugMode)
            {
                canvas.ResetTransform();
                canvas.TranslateTransform(xAttackLeft * 16, 24 * 16);
                canvas.FillRectangle(Brushes.Cyan, 0, 0, 32, 32);

                canvas.ResetTransform();
                canvas.TranslateTransform(xAttackRight * 16, 24 * 16);
                canvas.FillRectangle(Brushes.Cyan, 0, 0, 32, 32);

                canvas.ResetTransform();
                canvas.TranslateTransform(12 * 16, yAttackDown * 16);
                canvas.FillRectangle(Brushes.Cyan, 0, 0, 32, 32);

                for (int i = 0; i < 25; i++)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (peopleAttack[i, j] == 0)
                        {
                            canvas.ResetTransform();
                            canvas.TranslateTransform(i * 16, j * 16);
                            canvas.FillRectangle(Brushes.BlueViolet, 0, 0, 32, 32);
                        }
                        else if (peopleAttack[i, j] == 1)
                        {
                            canvas.ResetTransform();
                            canvas.TranslateTransform(i * 16, j * 16);
                            canvas.FillRectangle(Brushes.Blue, 0, 0, 32, 32);
                        }
                        else if (peopleAttack[i, j] == 2)
                        {
                            canvas.ResetTransform();
                            canvas.TranslateTransform(i * 16, j * 16);
                            canvas.FillRectangle(Brushes.Chocolate, 0, 0, 32, 32);
                        }
                    }
                }

                for (int i = 0; i < 25; i++)
                {
                    for (int j = 0; j < 25; j++)
                    {
                        if (bulletsDanger[i, j] > - 1)
                        {
                            canvas.ResetTransform();
                            canvas.TranslateTransform(i * 16, j * 16);
                            canvas.FillRectangle(Brushes.Red, 0, 0, 32, 32);
                        }
                    }
                }
            }


            /* <-----------------------------------------    Debug Mode Draw Code <----------------------------------------*/


            /*
             * Gọi đến phương thức vẽ nội của từng đối tượng xe tăng và đạn nằm trong danh sách "tanks" và "bullets"
            */
            foreach (var item in tanks)
            {
                item.Draw(canvas);
            }
            foreach (var item in bullets)
            {
                item.Draw(canvas);
            }

            /*------------------------------------------------> Draw Map ------------------------------------------> */
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    if (map[i, j] == Global.wall)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(256, 128, 16, 16), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.steel)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(272, 128, 16, 16), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.wall1)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(256, 128, 16, 8), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.wall2)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16 + 8);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(256, 128 + 8, 16, 8), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.wall3)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(256, 128, 8, 16), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.wall4)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16 + 8, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(256 + 8, 128, 8, 16), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.home)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(0, 128, 32, 32), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.homeDestroy)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(32, 128, 32, 32), GraphicsUnit.Pixel);
                    }
                    else if (map[i, j] == Global.tree)
                    {
                        canvas.ResetTransform();
                        canvas.TranslateTransform(j * 16, i * 16);
                        canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(365, 128, 16, 16), GraphicsUnit.Pixel);
                    } 
                }
            }
            /*<------------------------------------------------ Draw Map <------------------------------------------ */
            /*
             * Gọi đến phương thức vẽ của từng vụ nổ trong danh sách các vụ nổ "explosions"
            */
            foreach (var item in explosions.ToList())
            {
                item.Draw(canvas);
            }

            foreach (var item in items.ToList())
            {
                item.Draw(canvas);
            }
        }

        /*
         * Mục đích của hàm gameHelpDraw(Graphics canvas) là vẽ ra giao diện trợ giúp người chơi.
        */

        private void gameHelpDraw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 480);
        }

        /*
         * Mục đích của hàm gameIntroductionDraw(Graphics canvas) là vẽ ra giao diện thông tin về trò chơi.
        */

        private void gameIntroductionDraw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 480);

            canvas.ResetTransform();
            canvas.TranslateTransform(170, 20);
            canvas.DrawImage(Global.sprites, new Rectangle(0, 0, 93, 60), new Rectangle(0, 230, 280, 182), GraphicsUnit.Pixel);

            canvas.ResetTransform();
            canvas.DrawString("UNIVERSITY OF INFORMATION", fontSmall, Brushes.WhiteSmoke, 35, 97);
            canvas.DrawString("TECHNOLOGY", fontSmall, Brushes.WhiteSmoke, 135, 117);
            canvas.DrawString("Software Engineering Faculty", fontSmall, Brushes.WhiteSmoke, 42, 150);
            canvas.DrawString("--------------", fontSmall, Brushes.WhiteSmoke, 110, 173);
            canvas.DrawString("Intelligent Algorithms", fontSmall, Brushes.WhiteSmoke, 49, 198);
            canvas.DrawString("SE313.I21 FINAL PROJECT", fontSmall, Brushes.WhiteSmoke, 49, 218);
            canvas.DrawString("--------------", fontSmall, Brushes.WhiteSmoke, 110, 244);
            canvas.DrawString("INSTRUCTORS", fontSmall, Brushes.WhiteSmoke, 130, 263);
            canvas.DrawString("NGUYEN VINH KHA", fontSmall, Brushes.WhiteSmoke, 100, 288);
           
            canvas.DrawString("--------------", fontSmall, Brushes.WhiteSmoke, 110, 333);
            canvas.DrawString("GROUP MEMBERS", fontSmall, Brushes.WhiteSmoke, 113, 353);
            canvas.DrawString("NGUYEN VAN KHOA", fontSmall, Brushes.WhiteSmoke, 109, 380);
            canvas.DrawString("PHAN MINH HOANG", fontSmall, Brushes.WhiteSmoke, 106, 400);
          
            canvas.DrawString("--------------", fontSmall, Brushes.WhiteSmoke, 113, 440);
        }

        /*
         * Mục đích của hàm gameExitDraw(Graphics canvas) là vẽ ra giao diện của trò chơi khi chúng ta thoát trò chơi.
        */

        private void gameExitDraw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 480);

            canvas.DrawString("THANK YOU FOR PLAYING", fontSmall, Brushes.DarkOrange, 53, 200);
        }

        /*
         * Mục đích của hàm gameOverDraw(Graphics canvas) là vẽ ra giao diện của trò chơi khi chúng ta chơi thua.
        */

        private void gameOverDraw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(0, 0);
            canvas.FillRectangle(Brushes.Black, 0, 0, 416, 480);

            canvas.DrawString("GAME OVER", fontSmall, Brushes.DarkOrange, 140, 200);
        }

        /* <---------------------------------------------- Draw Method Code Area   <------------------------------------------- */

        public static int randomNumber(int a, int b)
        {
            int n = rnd.Next(b - a + 1);
            return n + a;
        }

        public int check4block16(int blockx, int blocky)
        {
            int result = 1, typeBlock;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    typeBlock = map[blocky + j, blockx + i];
                    if (typeBlock == Global.steel)
                    {
                        return 0;
                    }
                    if (typeBlock == Global.wall || typeBlock == Global.wall1 || typeBlock == Global.wall2 || typeBlock == Global.wall3 || typeBlock == Global.wall4)
                    {
                        result = 69;
                    }
                }
            }
            return result;
        }

        public bool checkBullet(int type1, int type2)
        { 
            int a = Global.non;
            int b = Global.tree;
            if (type1 == a && type2 == a)
            {
                return true;
            }
            if (type1 == b && type2 == b)
            {
                return true;
            }
            if (type1 == a && type2 == b)
            {
                return true;
            }
            if (type1 == b && type2 == a)
            {
                return true;
            }
            return false;
        }

        public int roundBlock(float coor)
        {
            return (int)Math.Round(coor / 16);
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (gameStatus == 1 && !gamePause)
            {
                if (e.KeyChar == (char)Keys.Escape)
                {
                    if (gamePause)
                    {
                        gamePause = false;
                    }
                    else
                    {
                        choiceMenu = 0;
                        gamePause = true;
                    }
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadFont();
        }

        private void loadFont()
        {
            byte[] fontArray = BattleCity.Properties.Resources.prstart;
            int dataLength = BattleCity.Properties.Resources.prstart.Length;
            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);
            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(ptrData, dataLength);
            Marshal.FreeCoTaskMem(ptrData);
            fontFamily = pfc.Families[0];
            fontLarge = new Font(fontFamily, 22f, FontStyle.Regular);
            fontSmall = new Font(fontFamily, 10f, FontStyle.Regular);
        }
    }
}
