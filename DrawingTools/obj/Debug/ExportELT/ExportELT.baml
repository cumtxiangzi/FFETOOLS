   M S B A M L   `   `   `  ����G  CDrawingTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.ExportELTWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  导出设备表��5   	   $	��140��6   $	��350��6$    � 
ResizeMode$ NoResize=�60    � WindowStartupLocation$ CenterScreen=�6G   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��120* �6   �� 5      $	��60* �6   �� 5      $	��60* �6   �� 5      $	��80* �6    � RowDefinitions 5   
   �� 5      $	��30* �6   �� 5      $	��20* �6   .+�5   
   �� $��请选择语言版本：��6   $��Center=�6$   $��10,0,0,2q�6A   $��Center=�6S   $��0��6n   $��0��6{   $��14(�6�   	� 5   
   -   5         	�Name$ 	CH_Button��5      $��中文��6)   $��Center=�66   $��0,0,0,0q�6S   $��Center=�6d   $��0��6   $��1��6�   $��14(�6�    d� 	IsChecked$
 TrueY�6�   	� 5   
   -   5        $ 	EN_Button��5      $��英文��6)   $��Center=�66   $��0,0,0,0q�6S   $��Center=�6d   $��0��6   $��2��6�   $��14(�6�   	� 5   
   -   5        $ CH_EN_Button��5      $��	中英文��6,   $��Center=�6:   $��0,0,0,0q�6W   $��Center=�6h   $��0��6�   $��3��6�   $��14(�6�   �� 5   
   -   5         ��Name$ OkButton��5      $��确定��6#   $��2��60   $��3��6=   $��	15,5,15,5q�6M   $��14(�6`    � Cursor$
 Hand��6n   ��5      �� 5      	 �� 
TargetType"	 ��6"   .R�5      �� #��z��6   $��1q�6Q   
 �� CornerRadius$
 3��6e   
�������._�5      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5       5!      5#      �� 5%   
   -   5         ��Name$ OnlyPipe��5%      $��仅导出管材管件��6%   $��1��67   $��0��6D   $��Center=�6T   $��12,5,0,5q�6o   $��13(�6�   5'      5(      