import React from 'react';
import styles from './styles.module.css'
import { MAX_HEIGHT, MAX_WIDTH } from "../../consts/sizes";


function getColorStyle(person) {
    if (person.personHealth == "Dying")
        return styles.dying;
    if(person.personHealth == "Sick")
        return styles.sick;
    return styles.healthy;

}

export default function Person({ person, onClick }) {
    const x = person.position.x / MAX_WIDTH * 100;
    const y = person.position.y / MAX_HEIGHT * 100;
    const colorStyle = getColorStyle(person)
    return (
        <div
            className={`${styles.root} ${colorStyle}`}
            style={{ left: `${ x }%`, top: `${ y }%` }}
            onClick={ () => onClick(person.id) }
        />
    );
}
