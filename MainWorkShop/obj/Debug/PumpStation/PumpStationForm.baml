   M S B A M L   `   `   `  ����G  CMainWorkShop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.PumpStationForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  创建泵房��5   	   $	��500��6   $	��500��6#    � WindowStartupLocation$ CenterScreen=�6/    � 
ResizeMode$ NoResize=�6T    � 	Resources 5	         
TextCenter �� 
TargetType �� Property �� Value��%�   &	        �� 5
   
   " ��6#   .�5      ��  � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       �@��TextAlignment$ Center=�6.   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $	��65* �6   �� 5      $
��150* �6   �� 5      $	��22* �6    � ColumnDefinitions 5   
   �� 5      $	��90* �6   �� 5      $	��15* �6   .+�5   
   �� $��泵房参数��6   $��0��6!   $��0��61   $��2��6>   $��5,5,5,5q�6R   
�������$��13(�6v   .��5      �  5      �� 5      $
��0.7* �6'   �� 5       $
��0.7* �6'   �� 5!      $��* �6'   �� 5"      $
��0.7* �6'    5&      �� 5'      $��* �6$   �� 5(      $��* �6$   .+�5+      �� $��泵房数量:��6   $��Center=�6)   $��Center=�6F   $��0��6a   $��1��6q   $��5,5,5,5q�6~   �� 5,      $��泵房宽(mm):��6   $��Center=�6,   $��Center=�6I   $��0��6d   $��0��6t   $��20,5,5,5q�6�   �� 5-      $��泵房顶部标高(m):��6   $��Center=�6.   $��Center=�6K   $��2��6f   $��0��6v   $��5,5,5,5q�6�   �� 5/      -   5        	 ��Name$	 RoomNumModifyBtn��5/      $��数量修改��63   $��25,10,55,10q�6B   $��13(�6W   
 � Cursor$

 Hand��6e   $��2��6�   $��1��6�   ��50      �� 51       �� 
TargetType" ��6*   .R�52      �� #��z��6%   $��1q�6Y    �� CornerRadius$ 3��6m   
�������._�54   "   �� #��z��63   $��Center=�6f   $��Center=�6�   6�   55      56      58      �� 5:      -   5         ��Name$ 	RoomMirro��5:      $��泵房镜像��6/   $��4��6>   $��1��6N   $��5,5,5,5q�6[   $��Center=�6l   $��13(�6�   6�   �� 5<      -   5         ��Name$ RoomNum��5<      $��3��6+   $��1��64   $��1��6D   $��5,5,5,5q�6Q   $��Center=�6b   $��13(�6~   5=   T   �� 5>      -   5        $ 	RoomWidth��5>      $
��6500��6-   $��1��69   $��0��6I   $��5,5,5,5q�6V   $��Center=�6g   $��13(�6�   �� 5?      -   5        $ 
RoomHeight��5?      $	��4.0��6.   $��3��69   $��0��6I   $��5,5,5,5q�6V   $��Center=�6g   $��13(�6�   5@      5A      �� 5C   
   $��房间参数��6   $��0��6!   $��1��61   $��2��6>   $��5,5,5,5q�6R   
�������$��13(�6v   .��5D      � $��0,5,0,0q�6   .+�5E      &   System.Windows.Controls.DataGrid  -   5          Name$ RoomSettingGrid��5E         AutoGenerateColumns .     CanUserAddRows .  $	��Top=�6h      CanUserReorderColumns .     CanUserResizeRows .     CanUserResizeColumns .     CanUserSortColumns .  $   HorizontalScrollBarVisibility$ Hidden=�5F   m   !   EnableColumnVirtualization .     EnableRowVirtualization .  $y�Hidden=�5G   ^      SelectionUnit$
 Cell=�5H   �      ColumnHeaderStyle 5I      �� 5J      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6    .�5K      ��  � ��$ Center=�6L   �� 5L       � �� 5M   "   �� 5N   &   .R�5O   *   �� $
��Wrap=�6F    �� TextAlignment$ Center=�6\   $��Center=�6v   ��64   �� 6v   5P   '   5R      5S         RowStyle 5V      �� 5W      )  #System.Windows.Controls.DataGridRow"  6    .�5X      ��  � ��$ 25��67   5Y         Columns 5\      0  *System.Windows.Controls.DataGridTextColumn  5^      ,  &System.Windows.Controls.DataGridColumn   Header$ 房间编号��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.5* 6]       
IsReadOnly  . 1  +System.Windows.Controls.DataGridBoundColumn!   ElementStyle#! [  6|   "   Binding" 6;   �� # �� Path$# RoomCode�6|   4  .System.Windows.Controls.DataGridTemplateColumn  5a      $ 房间名称��61   $ 1* 6?   $   CellTemplate$ 5c      �� 5d   "   .R�5e   &   �� % ��Name$% RoomName��60   $��Center=�5f   0   $��Center=�6K   ��5e   B   �� $# RoomNameList�& �� Mode$& TwoWay=�' �� UpdateSourceTrigger$' PropertyChanged=�5f   j   5g   #   (   CellEditingTemplate( 5j      �� 5k   "   .R�5l   &   �� ) ��Name$) ComboBoxRoomName��6/   -   5        * �� DisplayMemberPath$* RoomNameList��5n   /   + �� 
IsEditable+ . , �� Text, 5m   Q   �� $# RoomNameList�$& TwoWay=�$' PropertyChanged=�5n   d   5o   #   5q        5s      $ 房间长度(mm)��6-   $ 1* 5t   -   #! [  68   " 5s   ?   �� $# 
RoomLength�$& TwoWay=�$' PropertyChanged=�5t   8     5u      $ 房间底部标高(m)��6-   $ 1* 5v   -   #! [  68   " 5u   @   �� $# RoomBottomList�$& TwoWay=�$' PropertyChanged=�5v   8   5z      5{      5|      �� 5~   
   $*��$提示：房间排序为从左至右��6   $��
10,5,10,10q�6(   $��13(�6<   $��0��6J   $��2��6Z   $��Center=�6g   �� 5�   
   -	   5        $	 OkButton��5�      $��确 定��6#   $��	5,5,10,10q�61   $��12(�6D   $

 Hand��6R   $��1��6w   $��2��6�   ��5�      �� 5�      " ��6"   .R�5�      �� #��z��6   $��1q�6Q   $ 3��6e   
�������._�5�      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5�      5�      5�      5�      5�      