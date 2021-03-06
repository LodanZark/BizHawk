/******************************************************************************/
/* Mednafen Sony PS1 Emulation Module                                         */
/******************************************************************************/
/* negcon.h:
**  Copyright (C) 2012-2016 Mednafen Team
**
** This program is free software; you can redistribute it and/or
** modify it under the terms of the GNU General Public License
** as published by the Free Software Foundation; either version 2
** of the License, or (at your option) any later version.
**
** This program is distributed in the hope that it will be useful,
** but WITHOUT ANY WARRANTY; without even the implied warranty of
** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
** GNU General Public License for more details.
**
** You should have received a copy of the GNU General Public License
** along with this program; if not, write to the Free Software Foundation, Inc.,
** 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

#ifndef __MDFN_PSX_INPUT_NEGCON_H
#define __MDFN_PSX_INPUT_NEGCON_H

namespace MDFN_IEN_PSX
{
 InputDevice *Device_neGcon_Create(void);

	EW_PACKED(
	struct IO_NegCon
	{
		u8 buttons[2];
		u8 twist;
		u8 anabuttons[3];
		u8 active;
	});

}
#endif
