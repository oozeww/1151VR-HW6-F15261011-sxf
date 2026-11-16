Github鏈接： 

  https://github.com/oozeww/1151VR-HW6-F15261011-sxf

Youtube鏈接： 

  https://youtu.be/Hr0xcZdv0dQ

製作流程：

1.	創建九宮格地面，directional light，籃子控制，創建基礎的蘋果、炸彈掉落脚本

2.	創立prefab和控制其下落的脚本，給蘋果和炸彈添加tag，完善脚本

3.	添加UI和導演脚本關聯，顯示在屏幕上。

4.	調整關卡難度：物件掉落速度沒有修改，因爲攝像機距離比較近，凋落物下降距離短，加之速度太快會完全看不清；游戲有三個階段：normal，fast，frenzy，分別在40s、20s剩餘時間時切換，階段的區別主要在於物件生成的interval變短和bombRate的增加；爲了讓關卡有挑戰性，我設置游戲數值為蘋果一個+10，炸彈一個-50，這樣的游戲懲罰才足夠有挑戰，否則游戲分數很容易纍加
