   M S B A M L   `   `   `  ����F  BOutdoorPipe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.RaisePipesForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  管道升降��5   	   $	��150��6   $	��220��6#    � WindowStartupLocation$ CenterScreen=�6/    � 
ResizeMode$ NoResize=�6T   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��65* �6   �� 5      $	��60* �6   �� 5      $	��70* �6    � RowDefinitions 5   
   �� 5      $	��20* �6   �� 5      $	��15* �6   .+�5   
   �� $��管道升降高度(mm):��6   $��	10,15,5,5q�6'   $��0��6:   $��0��6J   $��2��6W   $��Center=�6k   $��13(�6�   �� 5   
   -   5         ��Name$ HeightValue��5      $��
10,15,10,5q�6'   $��2��6;   $��0��6K   $��Center=�6Y   $��13(�6t   �� 5   
   -   5         ��Name$ OkButton��5      $��确定��6#   $��
12,5,12,15q�60   $��13(�6D    � Cursor$
 Hand��6R   $��2��6`   $��2��6p   ��5      �� 5       �� 
TargetType" ��6"   .R�5      �� #��z��6   $��1q�6Q   	 �� CornerRadius$	 3��6e   
�������._�5      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5      5      5       5!      5"      