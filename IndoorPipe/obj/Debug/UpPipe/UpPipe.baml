   M S B A M L   `   `   `  ����E  AIndoorPipe, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:FFETOOLSFFETOOLS  5   	       FFETOOLS.UpPipeForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:FFETOOLS     5   	   -   5          � Title$  立管生成��5   	   $	��150��6   $	��200��6#    � WindowStartupLocation$ CenterScreen=�6/    � 
ResizeMode$ NoResize=�6T   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $	��65* �6   �� 5      $	��30* �6   �� 5      $	��70* �6    � RowDefinitions 5   
   �� 5      $	��20* �6   �� 5      $	��12* �6   .+�5   
   �� $��立管长度(mm):��6   $��9,15,5,5q�6%   $��0��67   $��0��6G   $��2��6T   $��Center=�6h   $��13(�6�   �� 5   
   -   5         ��Name$ LengthValue��5      $��	5,15,10,5q�6'   $��2��6:   $��0��6J   $��Center=�6X   $��13(�6s   �� 5   
   -   5         ��Name$ OkButton��5      $��确定��6#   $��	5,5,10,10q�60   $��13(�6C    � Cursor$
 Hand��6Q   $��2��6_   $��2��6o   ��5      �� 5       �� 
TargetType" ��6"   .R�5      �� #��z��6   $��1q�6Q   	 �� CornerRadius$	 3��6e   
�������._�5      �� #��z��6+   $��Center=�6^   $��Center=�6{   6�   5      5      5       5"      5#      