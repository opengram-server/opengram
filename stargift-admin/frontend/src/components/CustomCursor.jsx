import { useEffect } from 'react'

export default function CustomCursor() {
    useEffect(() => {
        const cursor = document.getElementById('custom-cursor')

        const moveCursor = (e) => {
            if (cursor) {
                cursor.style.left = `${e.clientX}px`
                cursor.style.top = `${e.clientY}px`
            }
        }

        const handleMouseDown = () => {
            if (cursor) cursor.classList.add('scale-75')
        }

        const handleMouseUp = () => {
            if (cursor) cursor.classList.remove('scale-75')
        }

        window.addEventListener('mousemove', moveCursor)
        window.addEventListener('mousedown', handleMouseDown)
        window.addEventListener('mouseup', handleMouseUp)

        return () => {
            window.removeEventListener('mousemove', moveCursor)
            window.removeEventListener('mousedown', handleMouseDown)
            window.removeEventListener('mouseup', handleMouseUp)
        }
    }, [])

    return null
}
