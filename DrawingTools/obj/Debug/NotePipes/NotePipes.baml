   M S B A M L   `   `   `  ����G  CDrawingTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.NotePipesForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  管道批量标注��5   
   $	��150��6   $	��250��6&    � WindowStartupLocation$ CenterScreen=�62    � 
ResizeMode$ NoResize=�6W   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��65* �6   �� 5      $	��60* �6   �� 5      $	��60* �6    � RowDefinitions 5   
   �� 5      $	��20* �6   �� 5      $	��12* �6   .+�5   
   �� $#��管道标注长度限值(mm):��6   $��9,15,5,5q�6)   $��0��6;   $��0��6K   $��2��6X   $��Center=�6l   $��13(�6�   �� 5   
   -   5         ��Name$ LengthValue��5      $��	5,15,10,5q�6'   $��2��6:   $��0��6J   $��Center=�6X   $��13(�6s   �� 5   
   -   5         ��Name$ OkButton��5      $��确定��6#   $��	5,5,10,10q�60   $��13(�6C    � Cursor$
 Hand��6Q   $��2��6_   $��2��6o   ��5      �� 5       �� 
TargetType" ��6"   .R�5      �� #��z��6   $��1q�6Q   	 �� CornerRadius$	 3��6e   
�������._�5      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5      5      5       5"      5#      