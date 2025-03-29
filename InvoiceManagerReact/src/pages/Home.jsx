//init home
import React, {useEffect} from 'react';

const Home = () => {
    useEffect(() => {
        document.title = 'Home - Invoice Manager';
        }, []);
    return (
        <div>
        <h1>Home</h1>
        </div>
    );
}

export default Home;