using System;

public enum Move {
	UP,
	DOWN,
	LEFT,
	RIGHT,
	STAY
}

public static class MoveUtils {
	public static Direction GetDirectionFor(Move move){
		switch (move) {
		case Move.UP:
			return Direction.NORTH;
		case Move.DOWN:
			return Direction.SOUTH;
		case Move.LEFT:
			return Direction.WEST;
		case Move.RIGHT:
			return Direction.EAST;
		default:
			throw new InvalidOperationException ();
		}
	}
}