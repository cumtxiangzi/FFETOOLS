   M S B A M L   `   `   `  ����F  BOutdoorPipe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.WellPointWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5        $��MainForm��5        � Title$  排水管网��6	   $	��600��6&   $	��700��63    � 
ResizeMode$ NoResize=�6?    � WindowStartupLocation$ CenterScreen=�5	   	   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��0.1* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��0.25* �6   �� 5      $��0.25* �6   �� 5      $��0.25* �6   .+�5   
   �� -   5         ��Name$ InputButton��5      $��导入管网数据��6&   $��	5,5,10,10q�67   $��12(�6J    � Cursor$
 Hand��6X   $��1��6�   $��1��6�   ��5      �� 5       �� 
TargetType" ��6"   .R�5      �� #��z��6   $��1q�6Q    �� CornerRadius$ 3��6e   
�������._�5      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5      5      5       �� 5"   
   -   5        $ CreatPipeButton��5"      $��生成排水管网��6*   $��	5,5,10,10q�6;   $��12(�6N   $
 Hand��6\   $��2��6�   $��1��6�   ��5#      �� 5$      " ��6"   .R�5%      �� #��z��6   $��1q�6Q   $ 3��6e   
�������._�5'      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5(      5)      5+      �� 5-   
   -   5        $ 
ExitButton��5-      $��取消��6%   $��	5,5,10,10q�62   $��12(�6E   $
 Hand��6S   $��3��6z   $��1��6�   ��5.      �� 5/      " ��6"   .R�50      �� #��z��6   $��1q�6Q   $ 3��6e   
�������._�52      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   53      54      56      �� 58   
   $��管网数据��6   
�������$��0��64   $��0��6A   $��4��6Q   $��5q�6f   .��59      � $��0,5,0,0q�6   .+�5:      &   System.Windows.Controls.DataGrid  -   5        	  Name$	 PS_List��5:      
   AutoGenerateColumns
 .  $
��Auto��6`   $��0,0,0,0q�6m   $	��Top=�6~      
IsReadOnly .    CanUserResizeColumns .     CanUserSortColumns .     CanUserResizeRows .     CanUserReorderColumns .  ��6,   ��    ColumnHeaderStyle 5<      �� 5=       �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6    .�5>      ��  �� Property � �� �� Value$ Center=�6K   5?      5@         Columns 5B      0  *System.Windows.Controls.DataGridTextColumn  5C      ,  &System.Windows.Controls.DataGridColumn   Header$ 	分组号��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.5* 6:   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6G   �� *Code+  5D      $ 排水井编号��6-   $ 1* 6<    6G   �� *Name+  5E      $ A坐标��6-   $ 1* 6:    6E   �� *Aaxis+  5F      $ B坐标��6-   $ 1* 6:    6E   �� *Baxis+  5G      $ 井面标高(m)��6-   $ 1* 6>    6I   �� *	UpLevel+  5H      $ 井底标高(m)��6-   $ 1* 6>    6I   �� *	DownLevel+  5I      $ 	井深(m)��6-   $ 1* 6<    6G   �� *Depth+5L      5N      5O      5P      5R      