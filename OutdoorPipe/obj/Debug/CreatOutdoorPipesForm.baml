   M S B A M L   `   `   `  ����F  BOutdoorPipe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	   $    FFETOOLS.CreatOutdoorPipesForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5        $
��this��5        � Title$  批量创建室外管道��6	   $	��480��6&   $	��900��63    � 
ResizeMode$ NoResize=�6?    � WindowStartupLocation$ CenterScreen=�6V    � 	Resources 5	         
TextCenter �� 
TargetType �� Property �� Value��%�   &	        �� 5
   
   " ��6#   .�5      ��  � ��$ Center=�64   �� 5       � ��$ Center=�62   �� 5       �@��TextAlignment$ Center=�6.   5      5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��160* �6   �� 5      $
��200* �6   �� 5      $��80 �6   �� 5      $��80 �6   �� 5      $��80 �6    � RowDefinitions 5   
   �� 5      $��65 �6   �� 5      $��* �6   �� 5      $��50 �6   .+�5    
   �� $��图示��6   
�������$��0��62   $��0��6B   $��3��6O   $��5q�6`   .��5!      �� $��5q�6   5"      �� 5$   
   $��批量创建管道��6   
�������$��1��66   $��0��6F   $��4��6S   $��5q�6g   .��5%      � $��0q�6    5&      �� 5'      $
��0.4* �6'   �� 5(      $��* �6'   �� 5)      $
��0.6* �6'   �� 5*      $��* �6'   .+�5,      �� $��专业:��6   $��Center=�6'   $��Center=�6D   $��0��6_   $��0��6o   $��5q�6|   �� 5-      $��管道数量:��6   $��Center=�6)   $��Center=�6F   $��2��6a   $��0��6q   $��5q�6~   �� 5.      -   5        	 ��Name$	 ProfessionCmb��5.      $��1��62   $��0��6B   $��5,5,30,5q�6O   �� 5/      -   5        
 ��Name$
 PipeQuantityTxt��5/      $��3��63   $��0��6C   $��5,5,30,5q�6P   6�   50      51      �� 53   
   $��管道设置��6   
�������$��1��64   $��1��6D   $��4��6Q   $��5q�6e   .��54      � $��0,5,0,0q�6   .+�57      &   System.Windows.Controls.DataGrid  -   5          Name$ PipeSettingGrid��57         AutoGenerateColumns .     CanUserAddRows .  $	��Top=�6�      CanUserReorderColumns .     CanUserResizeRows .     CanUserResizeColumns .     CanUserSortColumns .  ��64   ��    ColumnHeaderStyle 5:      �� 5;      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6    .�5<      ��  � ��$ Center=�6L   �� 5=       � �� 5>   "   �� 5?   &   .R�5@   *   �� $
��Wrap=�6F    �� TextAlignment$ Center=�6\   $��Center=�6v   ��64   �� 6v   5A   '   5C      5D         RowStyle 5G      �� 5H      )  #System.Windows.Controls.DataGridRow"  6    .�5I      ��  � ��$ 25��67   5J         Columns 5M      0  *System.Windows.Controls.DataGridTextColumn  5O      ,  &System.Windows.Controls.DataGridColumn   Header$ 管道编号��6-      Width5  /System.Windows.Controls.DataGridLengthConverter$
 0.8* 6]      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [  6|      Binding 6;   ��  �� Path$ PipeCode�6|   4  .System.Windows.Controls.DataGridTemplateColumn  5R      $ 管道系统��61   $
 1.9* 6?      CellTemplate 5T      �� 5U   "   .R�5V   &   ��  ��Name$ TextPipeSystem��60   $��Center=�6�   $��Center=�6�   ��6H   �� $ 
PipeSystem� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   5W   #       CellEditingTemplate  5Z      �� 5[   "   .R�5\   &   �� $	 ComboBoxPipeSystem��6/   -   5        ! �� DisplayMemberPath$! 
PipeSystem��5\   �   " �� Text" 6�   �� $ 
PipeSystem�$ TwoWay=�$ PropertyChanged=�6  5]   #   5_        5b      $ 管道类型��61   $
 1.4* 6?    5d      �� 5e   "   .R�5f   &   �� $ TextPipeType��60   $��Center=�6�   $��Center=�6�   ��6F   �� $ PipeType�$ TwoWay=�$ PropertyChanged=�6�   5g   #     5j      �� 5k   "   .R�5l   &   �� $	 ComboBoxPipeType��6/   -   5        $! PipeType��5l   �   " 6�   �� $ PipeType�$ TwoWay=�$ PropertyChanged=�6�   5m   #   5o        5r      $ 管道直径��61   $
 0.8* 6?    5t      �� 5u   "   .R�5v   &   �� $ TextPipeSize��60   $��Center=�6�   $��Center=�6�   ��6F   �� $ PipeSize�$ TwoWay=�$ PropertyChanged=�6�   5w   #     5z      �� 5{   "   .R�5|   &   �� $	 ComboBoxPipeSize��6/   -   5        $! PipeSize��5|   �   " 6�   �� $ PipeSize�$ TwoWay=�$ PropertyChanged=�6�   5}   #   5        5�      $ 管道高度(mm)��6-   $ 1* 6�   # [  6�    6?   �� $ 
PipeHeight�$ TwoWay=�$ PropertyChanged=�6�     5�      $" 管道与中心线间距(mm)��6-   $ 1* 6�   # [  6�    6C   �� $ PipeDistance�$ TwoWay=�$ PropertyChanged=�6�   5�      5�      5�      �� 5�   
   -   5        $��确定��5�      $��2��6   $��2��6/   $��12q�6<   ��5�      �� 5�      # �� 
TargetType"# ��6"   .R�5�      �� #��z��6   $��1q�6Q   $ �� CornerRadius$$ 3��6e   ���	#FFDDDDDD._�5�      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5�      5�      5�      �� 5�   
   -	   5        $��取消��5�      $��3��6   $��2��6/   $��12q�6<   ��5�      �� 5�      "# ��6"   .R�5�      �� #��z��6   $��1q�6Q   $$ 3��6e   ���	#FFDDDDDD._�5�      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5�      5�      5�      �� 5�   
   -
   5        $��间距计算��5�      $��4��6!   $��2��61   $��12q�6>   ��5�      �� 5�      "# ��6"   .R�5�      �� #��z��6   $��1q�6Q   $$ 3��6e   ���	#FFDDDDDD._�5�      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5�      5�      5�      5�      5�      